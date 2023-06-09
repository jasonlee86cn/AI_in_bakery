import torch

import os
import sys
import shutil
import sys

#if len(sys.argv)<2:
#    print("Fatal: You forgot to include the directory name on the command line.")
#    print("Usage:  python %s <File name>" % sys.argv[0])
#    sys.exit(1)

#gpus = sys.argv[1]

#if gpus == '': gpus = 'data/temp/temp.jpg'

#print(gpus)

# Model
model = torch.hub.load('ultralytics/yolov5', 'yolov5s')
#model = torch.hub.load('ultralytics/yolov5', 'custom', path='Z:\last.pt')  # local model
#model = torch.hub.load('path/to/yolov5', 'custom', path='path/to/best.pt', source='local')  # local repo

model.conf = 0.4  # NMS confidence threshold
model.iou = 0.45  # NMS IoU threshold
model.agnostic = False  # NMS class-agnostic
model.multi_label = False  # NMS multiple labels per box
model.classes = None  # (optional list) filter by class, i.e. = [0, 15, 16] for COCO persons, cats and dogs
model.max_det = 1000  # maximum number of detections per image
model.amp = False  # Automatic Mixed Precision (AMP) inference

# Image
#im = 'https://tmbidigitalassetsazure.blob.core.windows.net/rms3-prod/attachments/37/1200x1200/Lemon-Pound-Cake-Muffins_EXPS_BMZ18_31163_C12_08_2b.jpg'
#im = 'https://www.justataste.com/wp-content/uploads/2016/03/blueberry-coffee-cake-muffins-streusel-recipe.jpg'

#im = 'https://www.cookingwithmykids.co.uk/wp-content/uploads/2019/11/banana-bread-muffins-11-2-595x397.jpg'

im = 'data/temp/temp.jpg'

#im = gpus

#shutil.rmtree('data/output/images/')

work_dir = 'data/output'


try:
    shutil.rmtree(work_dir)
except OSError as e:
    print("Error: %s - %s." % (e.filename, e.strerror))

#work_dir_detect = 'C:\Python\yolov5\runs\detect'
#try:
#    shutil.rmtree(work_dir_detect)
#except OSError as e:
#    print("Error: %s - %s." % (e.filename, e.strerror))


# Inference
results = model(im)
results.save(save_dir=work_dir + '/save') 
results.crop(save_dir=work_dir + '/crop')

print(results.pandas().xyxy[0].to_json(orient="records"))

#print(results.pandas().xyxy[0])

crops = results.crop(save=True)
