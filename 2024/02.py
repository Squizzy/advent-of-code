list1: list[int] = []
list2: list[int] = []

with open("input") as f:
    lines = f.readlines()

    for line in lines:
        n1, n2 = line.strip().split("   ")
        list1.append(int(n1))
        list2.append(int(n2))

    list1.sort()
    list2.sort()

total = 0
for list_val in list1:
    total += list_val * sum(1 for a in list2 if a == list_val)

print (total)