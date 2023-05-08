# Power-Control-Panel-v2

Hello everyone! This is being dropped for support of my newest software, located here https://github.com/project-sbc/Handheld-Control-Panel


# PLEASE READ THIS:
THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. MISUSE OF THIS SOFTWARE COULD CAUSE SYSTEM INSTABILITY OR MALFUNCTION.

# Fix Error on Intel Devices Running Windows 11 22H2

Go to RegEdit to the following address:  Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CI\Config
change the DWORD value to 0 like the screenshot below.
Restart the computer

![Intel22H2](https://github.com/project-sbc/Power-Control-Panel-v2/blob/master/Intel%2022H2%20driver%20fix.jpg?raw=true "Intel 22H2 fix")


# Requirements:
You need .net 6.0 desktop to run this software OR download the self contained zip in the releases (newer versions only).

# Download
You can find it in the releases tab here https://github.com/project-sbc/Power-Control-Panel-v2/releases

# Compatible Devices (Windows only)
## Aya
-Aya Neo/Pro/Retro/Next/Air/Air Pro

## One Netbook
-One Mix 4, T1, OneGx/Pro
-One X Player AMD 4800U/5700U/5800U/6800U mini and big; Intel 1165G7/1195G7/1260p mini and big

## GPD
-Win 2, Win 3, Win max 2020, Win Max 2021 Intel/AMD, Win Max 2 1260p, Win Max 2 6800U Pocket 2, Pocket 3

## Anbernic
-Win600

## Intel Laptops
-4th generation or newer laptops or mini PCs

## AMD Laptops
-Most ryzen laptops or mini PCs

# Controller Shortcuts:
QUICK ACCESS MENU:   LB+RB+DPAD RIGHT  Hold for 1 second
KEYBOARD:   LB+RB+DPAD DOWN  Hold for 1 second

# Configure RTSS Frame Limit Feature
![RTSS](https://github.com/project-sbc/Power-Control-Panel-v2/blob/master/RTSS%20PCP%20Setup.jpg?raw=true "RTSS Setup")


# Features that are working:
-TDP changing for both AMD and Intel, including 12th gen alder lake
-TDP MSR changing for those intel devices that are problematic going above a certain TDP
-Volume and brightness (does not auto adjust, working on re-implementing that)
-Changing resolution, refresh rate, and windows scaling
-Quick Access Menu
-Customizable theme
-Auto start
-Minimize to tray


# Donate
If you would like to donate you can at:
https://ko-fi.com/project_sbc

https://www.paypal.com/donate?business=NQFQSSJBTTYY4&currency_code=USD
