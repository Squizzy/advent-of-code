list1: list[int] = []
list2: list[int] = []

with open("01_path.txt") as f:
    lines = f.readlines()

    # print(lines[0])
    # print(lines[1])
    for line in lines:
        n1, n2 = line.strip().split("   ")
        list1.append(int(n1))
        list2.append(int(n2))

    list1.sort()
    list2.sort()

    # dist = [b-a for a, b in zip(list1, list2)]
    # for d in dist:
    #     print("error") if d < 0 else print("ok")


# print(len(list1), len(list2))
print(sum(dist := [abs(b-a) for a, b in zip(list1, list2)]))