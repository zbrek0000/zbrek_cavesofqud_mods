import xml.etree.ElementTree as ET
from xml.dom import minidom

def parse_items(text, amount = False):
    items = {}

    for raw in text.splitlines():
        line = raw.strip()
        if not line:
            continue

        if line.startswith("- "):
            tier = line[2:].strip()
            items[tier] = {}
        elif line.startswith("# "):
            group = line[2:].strip()
            items[tier][group] = {}
        elif line.startswith("x "):
            name = " ".join([name[0].upper() + name[1:] for name in line[2:].strip().split()])
            items[tier][group][name] = {"exclude": True}
        else:
            if amount:
                *name_parts, bits, amount = line.split()
                name = " ".join([name[0].upper() + name[1:] for name in name_parts])
                items[tier][group][name] = {"bits": bits, "amount": amount}
            else:
                *name_parts, bits = line.split()
                name = " ".join([name[0].upper() + name[1:] for name in name_parts])
                items[tier][group][name] = {"bits": bits}

    return items


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

def items_to_list(items, name):
    for game_stage in items:
        for group in items[game_stage]:
            for item in items[game_stage][group]:
                python_obj = items[game_stage][group][item]
                if not "exclude" in python_obj:
                    amount = ""
                    if "amount" in python_obj:
                        amount = f"x{str(python_obj["amount"])}"
                    print(f"{item} {amount} ({python_obj["bits"]})")


# files = ["armor.txt", "bows.txt", "arrows.txt", "melee.txt", "other.txt"]
# print("[code]")
# for filename in files:
#     with open(filename) as f:
#         name = filename[:-4]
#         if name == "arrows":
#             items = parse_items(f.read(), True)
#         else:
#             items = parse_items(f.read())
#         items_to_list(items, name)
# print("[/code]")
# quit()

ARMOR = True
BOWS = True
ARROWS = True
MELEE = True
OTHER = True

if ARMOR:
    with open("armor.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"armor/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"armor/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if BOWS:
    with open("bows.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"bows/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"bows/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if ARROWS:
    with open("arrows.txt") as f:
        items = parse_items(f.read(), True)
        for game_stage in items:
            with open(f"arrows/{game_stage}/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if MELEE:
    with open("melee.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"melee/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"melee/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))

if OTHER:
    with open("other.txt") as f:
        items = parse_items(f.read())
        for game_stage in items:
            with open(f"other/{game_stage}/disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, True))
            with open(f"other/{game_stage}/no_disassemble/Objectblueprints.xml", "w+") as new_f:
                new_f.write(items_to_xml(items, game_stage, False))