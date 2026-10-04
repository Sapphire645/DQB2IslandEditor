substring = "roof"       # the substring to search for
new_value = "1"        # the new 3rd number
input_file = "items.txt"
output_file = "itemsnew.txt"

with open(input_file, "r") as infile, open(output_file, "w") as outfile:
    for line in infile:
        parts = line.strip().split("\t")
        if len(parts) >= 4:
            name = parts[6]
            if substring in name.lower():
                parts[5] = new_value  # change 3rd number
        outfile.write("\t".join(parts) + "\n")
