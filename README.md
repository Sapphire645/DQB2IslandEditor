# DQB2IslandEditor
A save editor for Dragon Quest Builders 2 that focuses on island-oriented tools.

This program will feature 3(pending) separate editing tools combined into one.

## Programs included:
- A Chunk Editor
- A Minimap Editor
- A Values Editor, including things like gratitude, weather, time, and possibly storage.

On top of that, there'll be a minimap export tool included, and a tool that will create a full island top view that can also be exported.
### Credits:
The Chunk Editor program is based on the now deprecated [Chunk Editor +](https://github.com/Sapphire645/DQB2ChunkEditorPlus), and consecuently on [Mufago](https://github.com/Mugafo)'s [Chunk Editor](https://github.com/Mugafo/DQB2ChunkEditor), and also consecuently, on [Turtle Insect](https://github.com/turtle-insect)'s [Map flattener](https://github.com/turtle-insect/DQB2).

The Minimap Editor program will be based on [Sapphire645](https://github.com/Sapphire645)'s [Minimap Export Tool](https://github.com/Sapphire645/DQB2MinimapExporter) (That's me!).

The Values editor may borrow from [Turtle Insect](https://github.com/turtle-insect)'s [STGDAT editor](https://github.com/turtle-insect/DQB2) for storage and the [Chunk Editor +](https://github.com/Sapphire645/DQB2ChunkEditorPlus) for text editing.

There will be specific minimap/full map export options for [Default Krammer](https://github.com/default-kramer)'s [Hermit's Heresy](https://github.com/default-kramer/HermitsHeresy) to use as templates.
### Special thanks:
- [Turtle Insect](https://github.com/turtle-insect) (Who made the original save editors and reversed engineered basically everything)
- [Mufago](https://github.com/Mugafo) (Original chunk editor and item research)
- [Default Krammer](https://github.com/default-kramer) (Item and chunk research)
# IE Chunk Editor:
This tool will open and edit the respective STGDAT.

## Tools:
- Block editing (Set blocks. This includes editing the island borders)
- Item editing (Set items)
- Chunk editing (Create, move, and delete chunks)
### Navigation
The grid at the right displays the current chunk from a top-down view, at a specific Y layer. 
- Move up/down the layer with the orange arrows, or select a specific layer on the rightmost bar.
- Move around the chunks with the yellow arrows, or select a specific chunk in the "map" tab by clicking on it.
>For old chunk editor users, the chunk numbers used are now the virtual chunk numbers, meaning the arrows move around the map as you would expect them to.
- The buttons on the bottom left of the grid can be used to toggle on/off the items. This is useful for selecting blocks that overlay the items.
### Block editing
You can select any block from the menu, or select any block in your own chunk with the "select tool".
Click on the "Edit Values" panel to set the values you want for your selected block.
- Chisel: Click on any tile in the chunk to chisel it to the selected value on the "Edit Values" panel.
- Paint: Click on any tile in the chunk to set it to the current block you have selected, including the block type, the chisel value, and the "builder placed" value.
### Item editing
You can select any item from the menu, or select any item in your own chunk with the "select tool".
Click on the "Edit Values" panel to set the values you want for your selected item.
Items have what is called a "item shell", which is the block that will be set on the chunk before being overlayed with the item. These are tied to some of the item's functionality, so you'll be allowed to edit the shell block of the item in this tab too.
- Paint: Click on any tile in the chunk to set it to the current item and item shell you have selected, including information like rotation, and others.

More implementation for the items coming soon. It is currently in research (and not even coded in lol)
### Areas
The "Area" tool allows you to select an area of your island. This can be used for painting, but is most relevant for the "replace" panel.
- The replace panel will allow you to select one or various blocks you want to replace, and one or various blocks to replace to.
- You will also be allowed to set the entirety of an area to one block.
  
# IE Minimap Editor:
Not implemented

# IE Values Editor:
Not implemented
