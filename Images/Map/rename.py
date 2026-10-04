import os
import shutil

mapping_file = "C://Users//N-BOOM//Documents//GitHub//DQB2IslandEditor//Info//Items.txt"
image_folder = "Object"

with open(mapping_file, "r") as file:
    for line in file:
        index, image_id = line.strip().split("\t")[:2]
        index = index.zfill(4)
        image_id = image_id.zfill(4)
# Look for any file starting with the index (e.g., 12.jpg or 12.png)
        for filename in os.listdir(image_folder):
            name, ext = os.path.splitext(filename)
            
            if name == "o"+index:
                src = os.path.join(image_folder, filename)
                dst = os.path.join(image_folder,"o" + image_id + ext)

                # Rename (or overwrite if exists)
                shutil.move(src, dst)
                print(f"Renamed {filename} → {image_id + ext}")
                break
