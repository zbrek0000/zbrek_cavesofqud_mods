from utils import parse_items

DIR = "items/"

def items_to_list(items, name):
    print(f"# {name}")
    for game_stage in items:
        for group in items[game_stage]:
            for item in items[game_stage][group]:
                python_obj = items[game_stage][group][item]
                if not "exclude" in python_obj:
                    amount = ""
                    if "amount" in python_obj:
                        amount = f"x{str(python_obj['amount'])}"
                    print(f"{item} {amount} ({python_obj['bits']})")

files = [DIR+"armor.txt", DIR+"bows.txt", DIR+"arrows.txt", DIR+"melee.txt", DIR+"other.txt"]
print("[code]")
for filename in files:
    with open(filename) as f:
        name = filename.split("/")[-1][:-4]
        if name == "arrows":
            items = parse_items(f.read(), True)
        else:
            items = parse_items(f.read())
        items_to_list(items, name)
print("[/code]")
quit()