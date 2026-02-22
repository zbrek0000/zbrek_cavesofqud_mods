import xml.etree.ElementTree as ET
import os

from utils import parse_items

DIR = "items/"
item_list = []
files = [DIR+"armor.txt", DIR+"bows.txt", DIR+"arrows.txt", DIR+"melee.txt", DIR+"other.txt"]
PATH_TO_OBJECT_BLUEPRINTS = ""

for filename in files:
    with open(filename) as f:
        name = filename.split("/")[-1][:-4]
        if name == "arrows":
            items = parse_items(f.read(), True)
        else:
            items = parse_items(f.read())
        for game_stage in items:
            for type in items[game_stage]:
                item_list += [item for item in items[game_stage][type] if 'exclude' not in items[game_stage][type][item]]

for dirpath, dnames, fnames in os.walk(PATH_TO_OBJECT_BLUEPRINTS):
    for filename in fnames:
        if filename.endswith(".xml"):
            absolute_path = os.path.join(dirpath, filename)
            with open(absolute_path) as f:
                text = f.read()
                text = text.replace("&", "").replace("#", "")   # avoid parsing errors
                tree = ET.fromstring(text)
                for obj in tree:
                    inherits = obj.get("Inherits")
                    if inherits:
                        if inherits in item_list:
                            print("x", obj.get("Name"))
                            print("inherits", inherits)
