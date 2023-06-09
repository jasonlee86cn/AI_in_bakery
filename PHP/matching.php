<?php
/*
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);
*/

include_once("conn.php");

$s=$_REQUEST["s"];
$guid=$_REQUEST["guid"];

$register_asset_id=$_REQUEST["register_asset_id"];
$keywords=$_REQUEST["keywords"];
$register_asset_merchant_code=$_REQUEST["merchant_code"];
$shopping_cart_product_barcode=$_REQUEST["shopping_cart_product_barcode"];

$order_id=$_REQUEST["order_id"];

switch($s){
	case "order_status":
	
		$sql_p1=sprintf("SELECT * FROM shopping_orders where shopping_order_mcode='%s' and shopping_order_acode='%s' and shopping_order_id='%s';",$register_asset_merchant_code,$register_asset_id,$order_id);
		//echo $sql_p1;
		$result_p1 = $mysqli->query($sql_p1);
		
		if($result_p1->num_rows>0){
			$row_p1 = $result_p1->fetch_assoc();
			
			$sql_p3=sprintf("SELECT * FROM merchants where merchant_number='%s';",$register_asset_merchant_code);
			$result_p3 = $mysqli->query($sql_p3);
			$row_p3 = $result_p3->fetch_assoc();
			
			$msg=array("status"=>"OK","info"=>"OK");
			$msg=array_merge($msg,$row_p1,$row_p3);
		}else{
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"No order.");
		}
		
	break;
	case "order_status_detail":
	
		$sql_p1=sprintf("SELECT * FROM shopping_orders where shopping_order_mcode='%s' and shopping_order_acode='%s' and shopping_order_id='%s';",$register_asset_merchant_code,$register_asset_id,$order_id);
		//echo $sql_p1;
		$result_p1 = $mysqli->query($sql_p1);
		
		if($result_p1->num_rows>0){
			$row_p1 = $result_p1->fetch_assoc();
			
			$sql_p2=sprintf("SELECT * FROM shopping_orders_detail where shopping_orders_detail_mcode='%s' and shopping_orders_detail_acode='%s' and shopping_orders_detail_order_id='%s';",$register_asset_merchant_code,$register_asset_id,$order_id);
			$result_p2 = $mysqli->query($sql_p2);
			
			$count_r=0;
			while($row_p2 = $result_p2->fetch_assoc()){
				$data_r[$count_r]=$row_p2;
				$count_r++;
			}
			
			$sql_p3=sprintf("SELECT * FROM merchants where merchant_number='%s';",$register_asset_merchant_code);
			$result_p3 = $mysqli->query($sql_p3);
			$row_p3 = $result_p3->fetch_assoc();
			
			$msg=array("status"=>"OK","info"=>"OK","order_details"=>$data_r);
			$msg=array_merge($msg,$row_p1,$row_p3);
		}else{
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"No order.");
		}
		
	break;
	case "order_update":
		$paid_tag=$_REQUEST["paid_tag"];
		$print_tag=$_REQUEST["print_tag"];
		$sql_p1=sprintf("update shopping_orders set shopping_order_paid_tag='%s', shopping_order_print_tag='%s' where shopping_order_mcode='%s' and shopping_order_acode='%s' and shopping_order_status<>'Completed' and shopping_order_id='%s';",
			$paid_tag,
			$print_tag,
			$register_asset_merchant_code,
			$register_asset_id,
			$order_id
		);
		$result_p1 = $mysqli->query($sql_p1);
		
		if($result_p1){
			$msg=array("status"=>"OK","info"=>"OK","order_id"=>$order_id);
			
		}else{
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Err.");
			
		}
		
	break;
	case "made_order":
		$c_url_1=sprintf("http://matching.p20220916.lipanlong.com/?register_asset_id=%s&merchant_code=%s&s=view",$register_asset_id,$register_asset_merchant_code);
		$r_url_1=file_get_contents($c_url_1);
		$r_url_1=json_decode($r_url_1,true);
		if($r_url_1["status"]=="OK"){
			$r_data=$r_url_1["data"];
			
			$shopping_orders_detail_order_id=date("YmdHis")."-".rand(100000,999999);
			$shopping_orders_detail_time=date("Y-m-d H:i:s");
			$shopping_order_amount=0;
			
			$c1=0;
			foreach($r_data as $r_data_k=>$r_data_v){
				
				
				$shopping_orders_detail_mcode=$r_data_v["shopping_cart_merchant_code"];
				$shopping_orders_detail_acode=$r_data_v["shopping_cart_asset_id"];
				$shopping_orders_detail_product_barcode=$r_data_v["shopping_cart_product_barcode"];
				$shopping_orders_detail_product_quantity=$r_data_v["shopping_cart_product_quantity"];
				
				$sql_p0=sprintf("SELECT product_price,product_name FROM products where merchant_code='%s' and product_barcode='%s' limit 1;",$shopping_orders_detail_mcode,$shopping_orders_detail_product_barcode);
				$result_p0 = $mysqli->query($sql_p0);
				$row_p0 = $result_p0->fetch_assoc();
				
				
				$shopping_orders_detail_product_price=$row_p0["product_price"];
				$shopping_orders_detail_product_name=$row_p0["product_name"];
				
				$shopping_orders_detail_product_price_subtotal=sprintf("%1.2f",$shopping_orders_detail_product_price*$shopping_orders_detail_product_quantity);
				
				
				$sql_p1=sprintf("insert into shopping_orders_detail (shopping_orders_detail_order_id, shopping_orders_detail_time, shopping_orders_detail_mcode, shopping_orders_detail_acode, shopping_orders_detail_product_barcode, shopping_orders_detail_product_quantity, shopping_orders_detail_product_price, shopping_orders_detail_product_name,shopping_orders_detail_product_price_subtotal) values ('%s','%s','%s','%s','%s','%1.6f','%1.2f','%s','%1.2f');",
					$shopping_orders_detail_order_id,
					$shopping_orders_detail_time,
					$shopping_orders_detail_mcode,
					$shopping_orders_detail_acode,
					$shopping_orders_detail_product_barcode,
					$shopping_orders_detail_product_quantity,
					$shopping_orders_detail_product_price,
					$shopping_orders_detail_product_name,
					$shopping_orders_detail_product_price_subtotal
				);
				$result_p1 = $mysqli->query($sql_p1);
				
				$shopping_order_amount=sprintf("%1.2f",$shopping_order_amount+$shopping_orders_detail_product_price*$shopping_orders_detail_product_quantity);
				$c1=$c1+$shopping_orders_detail_product_quantity;
			}
			
			
			if($result_p1){
				$shopping_order_id=$shopping_orders_detail_order_id;
				$shopping_order_time=date("Y-m-d H:i:s");
				$shopping_order_mcode=$shopping_orders_detail_mcode;
				$shopping_order_acode=$shopping_orders_detail_acode;
				$shopping_order_status="Pending";
				//$shopping_order_amount
				$shopping_order_items=$c1;
				$shopping_order_paid_tag="N";
				$shopping_order_member_code="";
				
				$sql_p2=sprintf("insert into shopping_orders (shopping_order_id, shopping_order_time, shopping_order_mcode, shopping_order_acode, shopping_order_status, shopping_order_amount, shopping_order_items, shopping_order_paid_tag, shopping_order_member_code) values ('%s','%s','%s','%s','%s','%s','%s','%s','%s');",
					$shopping_order_id,
					$shopping_order_time,
					$shopping_order_mcode,
					$shopping_order_acode,
					$shopping_order_status,
					$shopping_order_amount,
					$shopping_order_items,
					$shopping_order_paid_tag,
					$shopping_order_member_code
				);
				
				$result_p2 = $mysqli->query($sql_p2);
				
				if($result_p2){
					$msg=array("status"=>"OK","info"=>"OK","order_id"=>$shopping_order_id);
					$c_url_1=sprintf("http://matching.p20220916.lipanlong.com/?register_asset_id=%s&merchant_code=%s&s=del",$register_asset_id,$register_asset_merchant_code);
					$r_url_1=file_get_contents($c_url_1);
					$r_url_1=json_decode($r_url_1,true);
					if($r_url_1["status"]=="OK"){
						
					}
				}else{
					$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Error.");
					
				}
				
				
			}else{
				$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Error.");
				
			}
			

			
			
			
		}else{
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"No items found from shopping cart (p).");
		}
		
		
	break;
	case "view":
		$sql_p1=sprintf("SELECT * FROM shopping_carts where shopping_cart_asset_id='%s' and shopping_cart_merchant_code='%s';",$register_asset_id,$register_asset_merchant_code);
		$result_p1 = $mysqli->query($sql_p1);
		
		if($result_p1->num_rows>0){
			$c1=0;
			while($row_p1 = $result_p1->fetch_assoc()){
				
				$sql_p2=sprintf("SELECT product_name, product_price, product_image_id, product_barcode FROM products where product_barcode='%s';",$row_p1["shopping_cart_product_barcode"]);
				$result_p2 = $mysqli->query($sql_p2);
				$row_p2 = $result_p2->fetch_assoc();
				
				
				$data[$c1]=array_merge($row_p1,$row_p2);
				$c1++;
			}
			
				$msg=array("status"=>"OK","info"=>"OK","data"=>$data);
				//array_push($msg,$row_p1);
				//$msg = array_merge($msg, $row_p1);
			
			
			
		}else{
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"No items found.");
		}
		
	break;
	case "del":
		$sql_p0=sprintf("SELECT * FROM unique_identifiers where unique_identifier_1='%s' limit 1;",$guid);
		$result_p0 = $mysqli->query($sql_p0);
		if($result_p0->num_rows==0){
			
			$sql_p1=sprintf("delete FROM shopping_carts where shopping_cart_asset_id='%s';",$register_asset_id);
			$result_p1 = $mysqli->query($sql_p1);
			
			if($result_p1){
				$msg=array("status"=>"OK");
				$sql_p3=sprintf("insert into unique_identifiers (unique_identifier_time_created,unique_identifier_1) values (NOW(),'%s');",$guid);
				$result_p3 = $mysqli->query($sql_p3);
				
			}else{
				$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Internal error.");
			}
			
		}else{
			$msg=array("status"=>"OK","info"=>"du");
		}
	break;
	case "add":
		$sql_p0=sprintf("SELECT * FROM unique_identifiers where unique_identifier_1='%s' limit 1;",$guid);
		$result_p0 = $mysqli->query($sql_p0);
		if($result_p0->num_rows==0){
			$sql_p1=sprintf("SELECT * FROM shopping_carts where shopping_cart_asset_id='%s' and shopping_cart_product_barcode='%s' limit 1;",$register_asset_id,$shopping_cart_product_barcode);
			$result_p1 = $mysqli->query($sql_p1);
				
			if($result_p1->num_rows==0){
				$sql_p2=sprintf("insert into shopping_carts (shopping_cart_merchant_code,shopping_cart_asset_id, shopping_cart_product_barcode,shopping_cart_product_quantity, shopping_cart_time_added) values ('%s','%s','%s',1,now());",$register_asset_merchant_code,$register_asset_id,$shopping_cart_product_barcode);
				
				
			}else{
				$sql_p2=sprintf("update shopping_carts set shopping_cart_product_quantity=shopping_cart_product_quantity+1,shopping_cart_time_added=now() where shopping_cart_asset_id='%s' and shopping_cart_product_barcode='%s';",$register_asset_id,$shopping_cart_product_barcode);
				
			}
			
			$result_p2 = $mysqli->query($sql_p2);
			if($result_p2){
				$msg=array("status"=>"OK");
				$sql_p3=sprintf("insert into unique_identifiers (unique_identifier_time_created,unique_identifier_1) values (NOW(),'%s');",$guid);
				$result_p3 = $mysqli->query($sql_p3);
			}else{
				$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Internal error.");
			}
		}else{
			$msg=array("status"=>"OK","info"=>"du");
		}

	break;
	default:
		if($register_asset_id=="" || $keywords==""){
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Please input asset ID and keywords.");
		}else{
			$keywords=str_replace(" ","",$keywords);
			$keywords2 = explode(',',$keywords);
			
			$t1="";
			$t2="";
			$product_barcode2="";
			foreach($keywords2 as $keywords2_k=>$keywords2_v){
				//echo $keywords2_v;
				//echo "\r\n<br>";
				/*
				
				*/
				
				$t1=$t1." product_keywords like '%".$keywords2_v."%' or ";
				
				
				if(validate_EAN13Barcode($keywords2_v)==true){
					$product_barcode2=$keywords2_v;
					$t2="product_barcode='".$keywords2_v."' or ";
				}else{
					$t2=$t2." product_barcode='".$keywords2_v."' or ";
				}
				
				
			}
			
			$sql_p1=sprintf("SELECT product_barcode,product_name,product_price,product_image_id FROM products,registers where products.merchant_code=registers.register_asset_merchant_code and register_asset_merchant_status='OK' and register_asset_merchant_code='%s' and register_asset_id='%s' and (%s %s product_barcode='X') order by if(LENGTH(product_barcode)=13,0,1),product_snid desc limit 1;",
			$register_asset_merchant_code,$register_asset_id,$t1,$t2);
			//echo $sql_p1;
			$result_p1 = $mysqli->query($sql_p1);
			
			if($result_p1->num_rows>0){
				$row_p1 = $result_p1->fetch_assoc();

				$msg=array("status"=>"OK","info"=>"OK","data"=>"");
				//array_push($msg,$row_p1);
				$msg = array_merge($msg, $row_p1);

			
				
			}else{
				$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Not item found.");
			}
		}
		
}

echo json_encode($msg,JSON_UNESCAPED_UNICODE);
mysqli_close($mysqli);



function validate_EAN13Barcode($barcode)

{

  // check to see if barcode is 13 digits long

  if(!preg_match("/^[0-9]{13}$/",$barcode)) {

     return false;

  }





  $digits = $barcode;





  // 1. Add the values of the digits in the even-numbered positions: 2, 4, 6, etc.

  $even_sum = $digits[1] + $digits[3] + $digits[5] + $digits[7] + $digits[9] + $digits[11];

  // 2. Multiply this result by 3.

  $even_sum_three = $even_sum * 3;

  // 3. Add the values of the digits in the odd-numbered positions: 1, 3, 5, etc.

  $odd_sum = $digits[0] + $digits[2] + $digits[4] + $digits[6] + $digits[8] + $digits[10];

  // 4. Sum the results of steps 2 and 3.

  $total_sum = $even_sum_three + $odd_sum;

  // 5. The check character is the smallest number which, when added to the result in step 4,  produces a multiple of 10.

  $next_ten = (ceil($total_sum/10))*10;

  $check_digit = $next_ten - $total_sum;





  // if the check digit and the last digit of the barcode are OK return true;

  if($check_digit == $digits[12]) {

    return true;

  } 





  return false;

}

?>