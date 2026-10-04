import zlib
from PIL import Image, ImageEnhance
from math import ceil, sqrt
Size = 64

def extract_tiles_from_image(image_path,TileSize = 64,tile_size = (64,64)):
    """
    Extract tiles from a given image.
    
    :param image_path: Path to the tile set image.
    :param tile_size: Size of each tile.
    :return: A list of Image objects representing individual tiles.
    """
    source_image = Image.open(image_path)
    img_width, img_height = source_image.size
    print("thing")
    tiles = []
    for y in range(0, img_height, TileSize):
        for x in range(0, img_width, TileSize):
            # Crop the tile from the image
            tile = source_image.crop((x, y, x +tile_size[0], y+tile_size[1]))
            tile.convert('RGBA')
            tiles.append(tile)
    return tiles

def create_icon_sheet(tiles1,tiles2,tiles3,tiles4,output_file):
    tiles = []
    maxnum = len(tiles1)*4
    for i in range(len(tiles1)):
        tiles.append(tiles1[i])
        tiles.append(tiles2[i])
        tiles.append(tiles3[i])
        tiles.append(tiles4[i])
    # Square grid layout (best for compactness)
    grid_size = ceil(sqrt(maxnum))
    sheet_width = grid_size * 64
    sheet_height = grid_size * 64
    grid_size = 32
    sheet_width = 32*64
    sheet_height = 36*64
    sheet = Image.new("RGBA", (sheet_width, sheet_height), (0, 0, 0, 0))

    for index, img in enumerate(tiles):
        x = (index % grid_size) * 64
        y = (index // grid_size) * 64
        sheet.paste(img, (x, y))

    sheet.save(output_file)
    print(f"Icon sheet created: {output_file}")
    print(f"Included {maxnum} icons in a {grid_size}x{grid_size} grid.")


OUTPUT_FILE = "tile_sheet.png"

def main():
    images1 = extract_tiles_from_image("MapSheet1.png")
    print("1")
    images2 = extract_tiles_from_image("MapSheet2.png")
    print("1")
    images3 = extract_tiles_from_image("MapSheet3.png")
    print("1")
    images4 = extract_tiles_from_image("MapSheet4.png")
    print("1")
    create_icon_sheet(images1,images2,images3,images4, OUTPUT_FILE)
    print("end")

    
if __name__ == "__main__":
    main()
