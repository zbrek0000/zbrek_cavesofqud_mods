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