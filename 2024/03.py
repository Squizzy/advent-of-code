report_list: list[int] = []
number_of_safe_reports: int = 0

def is_safe(report_list: list[int]) -> bool:

    increase: bool = (report_list[1] - report_list[0]) > 0

    for i in range(len(report_list) - 1):
        # No change: unsafe
        if report_list[i] == report_list[i + 1]: 
            return False
            
        # Change in direction: unsafe
        if ((report_list[i + 1] - report_list[i]) > 0) != increase: 
            return False
            
        # if > 3: unsafe
        if (abs(report_list[i + 1] - report_list[i]) > 3): 
            return False

    return True


def is_safe_with_one_less_level(report_list: list[int]) -> bool:
    
    for level in range(len(report_list)):
        
        reduced_report: list[int] = report_list.copy()
        reduced_report.pop(level)
        if is_safe(reduced_report):
            return True
            
    return False

rep_no: int = 0
safe_report: dict[int, tuple[bool, bool]] = {}

with open("02_reports.txt") as f:
    lines = f.readlines()

    for line in lines:
        report_list = [int(x) for x in line.strip().split()]
        
        if is_safe(report_list):
            number_of_safe_reports += 1
            print(f"report {rep_no} is safe")
            safe_report[rep_no] = (True, False)
        
        else:
            if is_safe_with_one_less_level(report_list):
                number_of_safe_reports += 1
                print(f"report {rep_no} is safe with one less level")
                safe_report[rep_no] = (False, True)
            else:
                print(f"report {rep_no} is NOT safe")
                safe_report[rep_no] = (False, False)
        
        rep_no += 1
            
    print (number_of_safe_reports)
            
with open("safe_reports.csv", "w") as f:
    f.write("Report Number, Safe First, Safe Second\n")
    for rep in safe_report:
        f.write(f"{rep}, {1 if safe_report[rep][0] else 0}, {1 if safe_report[rep][1] else 0}\n")
            
