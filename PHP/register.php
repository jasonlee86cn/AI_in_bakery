<?php
//ini_set('display_errors', 1);
//ini_set('display_startup_errors', 1);
//error_reporting(E_ALL);
$mysqli = new mysqli("database.p20220916.lipanlong.com", "p20220916_web", "p20220916_web..", "p20220916");
//print_r($mysqli);
/* check connection */
if (mysqli_connect_errno()) {
    printf("Connect failed: %s\n", mysqli_connect_error());
    exit();
}


/* change db to world db */
$mysqli->select_db("p20220916");
mysqli_query($mysqli,'set names utf8;');



$timenow=date("Y-m-d H:i:s");

$s=$_REQUEST["s"];
$register_asset_id=$_REQUEST["asset_id"];
$register_asset_merchant_code=$_REQUEST["merchant_code"];
$register_asset_merchant_name=$_REQUEST["merchant_name"];

switch($s){
	case "register":
		//http://register.p20220916.lipanlong.com/register.php?s=register&asset_id=1
		if($register_asset_id==""){
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Please input asset ID.");
		}else{
			$sql_p1=sprintf("SELECT * FROM registers where register_asset_id='%s' and (register_asset_merchant_status='Pending' or register_asset_merchant_status='OK');",$register_asset_id);
			$result_p1 = $mysqli->query($sql_p1);
			
			if($result_p1->num_rows>0){
				$row_p1 = $result_p1->fetch_assoc();
				if($row_p1["register_asset_merchant_status"]=="OK"){
					$msg=array("status"=>"OK","info"=>"OK","data"=>"");
					//array_push($msg,$row_p1);
					$msg = array_merge($msg, $row_p1);
				}else{
					$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Pending.");
				}
				
				
			}else{
				//$sql_p2=sprintf("insert into registers (register_asset_id, register_asset_merchant_status) values ('%s','Pending');",$register_asset_id);
				//$result_p2 = $mysqli->query($sql_p2);
				$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Not asset found. Please contact support.","qr_code"=>"http://register.p20220916.lipanlong.com/phpqrcode/text_qr.php?text=".$register_asset_id);
			}
		}
		
		
		
	break;
	case "new":
		
		if($register_asset_id=="" || $register_asset_merchant_code=="" || $register_asset_merchant_name==""){
			$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Asset ID or merchant code or name is missing.");
		}else{
			$sql_p1=sprintf("SELECT * FROM registers where register_asset_id='%s';",$register_asset_id);
			$result_p1 = $mysqli->query($sql_p1);
			
			
			if($result_p1->num_rows>0){
				$row_p1 = $result_p1->fetch_assoc();
				$msg=array("status"=>"ERR","info"=>"Error","error_reason"=>"Asset ID has registered.");
			}else{
				
				$register_asset_merchant_matching_url="http://matching.p20220916.lipanlong.com/";
				$register_asset_merchant_ai_url="http://ai.p20220916.lipanlong.com/";
				$register_asset_merchant_status="OK";
				$register_asset_merchant_timezone="NZT";
				$register_asset_merchant_time_added=$timenow;
				$register_asset_merchant_time_updated=$timenow;
				
				
				$sql_p2=sprintf("insert into registers (register_asset_id, register_asset_merchant_code, register_asset_merchant_name, register_asset_merchant_matching_url, register_asset_merchant_ai_url, register_asset_merchant_status, register_asset_merchant_timezone, register_asset_merchant_time_added, register_asset_merchant_time_updated) values ('%s','%s','%s','%s','%s','%s','%s','%s','%s');",
				$register_asset_id,$register_asset_merchant_code,$register_asset_merchant_name, $register_asset_merchant_matching_url, $register_asset_merchant_ai_url, $register_asset_merchant_status, $register_asset_merchant_timezone, $register_asset_merchant_time_added, $register_asset_merchant_time_updated);
				//echo $sql_p2;
				
				
				$result_p2 = $mysqli->query($sql_p2);
				if($result_p2){
					$msg=array("status"=>"OK","info"=>"OK","data"=>"");
				}else{
					$msg=array("status"=>"ERR","info"=>"Error","data"=>"");
				}
				
				
			}
			
			
		}
		
	break;
	default:

}

echo json_encode($msg,JSON_UNESCAPED_UNICODE);
mysqli_close($mysqli);
?>