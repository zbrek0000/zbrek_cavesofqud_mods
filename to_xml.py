import xml.etree.ElementTree as ET
from xml.dom import minidom
from utils import parse_items

DIR = "items/"

def items_to_xml(items: dict, game_stage: str, disassemble = False):
    root = ET.Element("objects")

    index = 1
    for group in items[game_stage]:
        comment = ET.Comment(f' === {group} === ')
        root.insert(index, comment)
        index += 1
        for item in items[game_stage][group]:
            python_obj = items[game_stage][group][item]
            obj = ET.SubElement(root, "object", {
                "Name": item,
                "Load": "Merge"
            })
            if "exclude" in python_obj:
                subelement = {
                    "Name": "TinkerItem",
                }
                ET.SubElement(obj, "removepart", subelement)
            else:
                subelement = {
                    "Name": "TinkerItem",
                    "Bits": python_obj["bits"],
                    "CanBuild": "true",
                    "CanDisassemble": "true" if disassemble else "false",
                }
                if "amount" in python_obj:
                    subelement["NumberMade"] = python_obj["amount"]
                ET.SubElement(obj, "part", subelement)
            index += 1

    rough = ET.tostring(root, encoding="utf-8")
    reparsed = minidom.parseString(rough)
    return reparsed.toprettyxml(indent="\t", encoding="utf-8").decode("utf-8")

ARMOR = True
BOWS = True
ARROWS = True
MELEE = True
OTHER = True

if ARMOR:
    with open(DIR+"armor.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"armor/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"armor/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if BOWS:
    with open(DIR+"bows.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"bows/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"bows/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if ARROWS:
    with open(DIR+"arrows.txt") as f:
        items = parse_items(f.read(), True)
        for game_stage in items:
            with open(f"arrows/{game_stage}/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if MELEE:
    with open(DIR+"melee.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"melee/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"melee/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if OTHER:
    with open(DIR+"other.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"other/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"other/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))