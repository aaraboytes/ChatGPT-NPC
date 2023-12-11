// Created by Crowbar 0.56

$modelname "_TEST-DRIVE/Fortnite_BR_Reaper/Fortnite_BR_Reaper.mdl"

$bodygroup "Fortnite_BR_Reaper"
{
	studio "fortnite_br_reaper_head_abs.smd"
}
$bodygroup "BODY"
{
	studio "fortnite_br_reaper_body.smd"
	blank
}


$surfaceprop "default"

$contents "solid"

$illumposition 0 0.292 39.544

$mostlyopaque

$cdmaterials "models\_TEST-DRIVE\Fortnite_BR_Reaper\"
$cdmaterials ""

$cbox 0 0 0 0 0 0

$bbox -11.592 -21.233 -4.332 11.007 21.232 83.419

$definebone "root" "" 0 0 0 0.43508 90.000003 90.000003 0 0 0 0 0 0
$definebone "bip_pelvis" "root" 0 46.560905 0 -0.000144 -19.807113 0.000208 0 0 0 0 0 0
$definebone "bip_spine_0" "bip_pelvis" -0.984766 3.243568 -0.000009 0.000197 18.117859 -0.000148 0 0 0 0 0 0
$definebone "Bip01_Spine" "bip_spine_0" 0 3.625919 0 0 -2.446287 0 0 0 0 0 0 0
$definebone "bip_spine_1" "Bip01_Spine" 0 3.005207 0 0 -1.704708 0 0 0 0 0 0 0
$definebone "bip_spine_2" "bip_spine_1" 0 4.051182 0 0 1.507785 0 0 0 0 0 0 0
$definebone "bip_spine_3" "bip_spine_2" 0 5.007309 0 0.000019 -0.230048 -0.000021 0 0 0 0 0 0
$definebone "bip_collar_L" "bip_spine_3" 0.058026 2.179192 1.161161 3.649388 -88.856862 86.866811 0 0 0 0 0 0
$definebone "bip_collar_R" "bip_spine_3" 0.05802 2.179237 -1.161163 -3.649311 -88.856807 -86.866825 0 0 0 0 0 0
$definebone "bip_neck" "bip_spine_3" -0.116045 5.65625 0.000002 -0.000026 23.507519 0.000013 0 0 0 0 0 0
$definebone "Bip01_Neck" "bip_neck" -0.000002 2.281334 0 0 -5.781345 0 0 0 0 0 0 0
$definebone "bip_head" "Bip01_Neck" 0 2.695694 0 -0.000001 -13.202152 0 0 0 0 0 0 0
$definebone "faceAttach" "bip_head" 0.095328 -0.494522 0.024219 -2.277922 -107.740149 -179.634714 0 0 0 0 0 0
$definebone "C_brow_mid" "faceAttach" -1.403961 6.386225 -0.053409 -0.499123 159.102223 -91.578508 0 0 0 0 0 0
$definebone "L_brow_mid" "faceAttach" -1.860664 6.124901 -1.694497 -0.568449 157.183555 -63.07453 0 0 0 0 0 0
$definebone "L_brow_outer" "faceAttach" -2.225197 5.202595 -2.701375 -5.207796 140.442119 -37.859032 0 0 0 0 0 0
$definebone "L_cheek_inner" "faceAttach" 0.58432 4.657261 -2.174038 14.794859 -157.740816 -32.930525 0 0 0 0 0 0
$definebone "C_lip_upper_mid" "faceAttach" 2.215622 5.065048 0.081624 2.278015 161.786296 -90.365213 0 0 0 0 0 0
$definebone "L_lip_upper_outer" "faceAttach" 2.107216 4.687902 -0.684801 2.278037 161.78631 -90.36522 0 0 0 0 0 0
$definebone "L_lip_corner" "faceAttach" 2.104927 4.437025 -1.167185 2.278019 161.786296 -90.365207 0 0 0 0 0 0
$definebone "C_nose_bridge" "faceAttach" 0.230087 6.563599 0.00403 2.477182 161.982924 -90.429609 0 0 0 0 0 0
$definebone "R_brow_mid" "faceAttach" -1.990585 6.1471 1.517367 10.100651 166.587496 -116.782152 0 0 0 0 0 0
$definebone "R_brow_outer" "faceAttach" -2.42997 5.224239 2.509294 25.643138 170.73618 -138.513533 0 0 0 0 0 0
$definebone "R_cheek_inner" "faceAttach" 0.414803 4.688351 2.203333 -49.759199 83.694284 -60.785112 0 0 0 0 0 0
$definebone "R_lip_corner" "faceAttach" 1.989281 4.439028 1.331777 2.27805 161.786351 -90.36522 0 0 0 0 0 0
$definebone "C_jaw" "faceAttach" 0.375198 1.59668 0.064323 0.210506 -32.052828 0.616182 0 0 0 0 0 0
$definebone "L_lip_lower_outer" "C_jaw" -0.200302 3.535824 -0.798852 2.334871 -166.133616 -89.716031 0 0 0 0 0 0
$definebone "C_lip_lower_mid" "C_jaw" -0.227081 3.913327 -0.024975 2.33485 -166.133643 -89.716045 0 0 0 0 0 0
$definebone "R_lip_lower_outer" "C_jaw" -0.211731 3.601919 0.819235 2.334871 -166.133616 -89.716031 0 0 0 0 0 0
$definebone "teeth_lower" "C_jaw" 0.467255 2.79752 0.028034 3.871664 -164.492376 -86.598945 0 0 0 0 0 0
$definebone "tongue" "C_jaw" 0.171844 0.909004 -0.023443 1.72138 -166.743443 -90.663092 0 0 0 0 0 0
$definebone "R_eye" "faceAttach" -1.823914 5.152065 1.52779 -73.968504 -102.353405 69.376759 0 0 0 0 0 0
$definebone "R_eye_lid_upper_mid" "R_eye" -0.027999 0.54583 0.248878 -74.351746 -107.836776 19.098773 0 0 0 0 0 0
$definebone "R_eye_lid_lower_mid" "R_eye" 0.027999 0.532272 -0.248886 -74.351008 -107.828723 19.090874 0 0 0 0 0 0
$definebone "L_eye" "faceAttach" -1.695786 5.131018 -1.679736 71.750113 -92.863 -56.735694 0 0 0 0 0 0
$definebone "L_eye_lid_upper_mid" "L_eye" -0.012938 0.536318 -0.2537 71.21671 -99.3968 171.418707 0 0 0 0 0 0
$definebone "L_eye_lid_lower_mid" "L_eye" 0.012939 0.537361 0.253731 71.215494 -99.398767 171.416849 0 0 0 0 0 0
$definebone "teeth_upper" "faceAttach" 1.35508 4.539431 0.05612 4.741608 164.113027 -93.001557 0 0 0 0 0 0
$definebone "R_lip_upper_outer" "faceAttach" 2.053497 4.726208 0.859381 2.27802 161.786296 -90.365227 0 0 0 0 0 0
$definebone "bip_upperArm_L" "bip_collar_L" -0.000008 7.755546 0 -64.110007 -55.143044 -10.640607 0 0 0 0 0 0
$definebone "bip_lowerArm_L" "bip_upperArm_L" 0 13.325668 0 -1.484441 -27.675218 7.550334 0 0 0 0 0 0
$definebone "bip_hand_L" "bip_lowerArm_L" 0 11.174576 0.000004 -44.805802 42.879621 -65.494582 0 0 0 0 0 0
$definebone "index_metacarpal_l" "bip_hand_L" 1.293496 0.833385 -0.7116 45.57803 4.034173 35.400652 0 0 0 0 0 0
$definebone "bip_index_0_L" "index_metacarpal_l" 0 4.150524 0 -17.53515 12.613483 -16.39999 0 0 0 0 0 0
$definebone "bip_index_1_L" "bip_index_0_L" 0 1.686218 0 -28.827654 34.332732 -38.380452 0 0 0 0 0 0
$definebone "bip_index_2_L" "bip_index_1_L" -0.000002 1.280415 -0.000001 -21.018159 28.971375 -12.721805 0 0 0 0 0 0
$definebone "middle_metacarpal_l" "bip_hand_L" 0.490374 1.008759 0.18197 43.588389 8.695258 38.39806 0 0 0 0 0 0
$definebone "bip_middle_0_L" "middle_metacarpal_l" 0.000002 3.665802 -0.000004 -9.188093 1.54594 -6.991882 0 0 0 0 0 0
$definebone "bip_middle_1_L" "bip_middle_0_L" -0.000001 2.006851 -0.000002 -33.656273 39.627822 -49.272312 0 0 0 0 0 0
$definebone "bip_middle_2_L" "bip_middle_1_L" 0 1.30521 -0.000002 -19.911499 23.169918 -12.96041 0 0 0 0 0 0
$definebone "pinky_metacarpal_l" "bip_hand_L" -1.222552 0.857586 1.281398 41.952863 18.66181 46.941667 0 0 0 0 0 0
$definebone "bip_pinky_0_L" "pinky_metacarpal_l" 0.000001 3.594009 -0.000002 -24.914777 3.228799 -21.294196 0 0 0 0 0 0
$definebone "bip_pinky_1_L" "bip_pinky_0_L" 0.000001 1.415386 0.000001 -32.079128 32.785431 -40.381715 0 0 0 0 0 0
$definebone "bip_pinky_2_L" "bip_pinky_1_L" -0.000002 1.102428 0 -17.969782 17.747863 -10.280184 0 0 0 0 0 0
$definebone "ring_metacarpal_l" "bip_hand_L" -0.251985 0.981663 0.829946 39.431044 12.752832 39.395702 0 0 0 0 0 0
$definebone "bip_ring_0_L" "ring_metacarpal_l" 0 3.529118 -0.000002 -12.580677 -1.923195 -9.277999 0 0 0 0 0 0
$definebone "bip_ring_1_L" "bip_ring_0_L" 0.000001 1.82428 0 -34.061163 36.434494 -46.379949 0 0 0 0 0 0
$definebone "bip_ring_2_L" "bip_ring_1_L" 0.000001 1.208992 0 -17.106969 15.870168 -11.049971 0 0 0 0 0 0
$definebone "bip_thumb_0_L" "bip_hand_L" 0.956927 0.749863 -0.699533 -27.262371 5.408823 -25.010792 0 0 0 0 0 0
$definebone "bip_thumb_1_L" "bip_thumb_0_L" 0 2.966976 -0.000004 17.049595 11.929514 25.755522 0 0 0 0 0 0
$definebone "bip_thumb_2_L" "bip_thumb_1_L" -0.000003 1.127224 -0.000001 6.023671 5.108061 9.399505 0 0 0 0 0 0
$definebone "lowerarm_twist_01_l" "bip_lowerArm_L" -0.000002 8.927441 0.000008 0.000002 -0.000006 0.000003 0 0 0 0 0 0
$definebone "lowerarm_twist_02_l" "bip_lowerArm_L" 0 4.809944 0.000008 -0.000003 -0.00001 -0.000004 0 0 0 0 0 0
$definebone "upperarm_twist_01_l" "bip_upperArm_L" 0 0.002041 0.000006 0.00635 -0.006671 0.013343 0 0 0 0 0 0
$definebone "upperarm_twist_02_l" "bip_upperArm_L" 0 6.897049 0.000002 -0.000003 -0.000015 -0.000022 0 0 0 0 0 0
$definebone "bip_upperArm_R" "bip_collar_R" -0.000008 7.755537 0 64.110069 -55.143137 10.640585 0 0 0 0 0 0
$definebone "bip_lowerArm_R" "bip_upperArm_R" 0 13.325718 -0.000002 1.484434 -27.675215 -7.550363 0 0 0 0 0 0
$definebone "bip_hand_R" "bip_lowerArm_R" 0 11.174606 0.000004 44.805416 42.887277 65.502635 0 0 0 0 0 0
$definebone "index_metacarpal_r" "bip_hand_R" 1.293402 0.833366 0.711762 -45.582538 4.034425 -35.404703 0 0 0 0 0 0
$definebone "bip_index_0_R" "index_metacarpal_r" -0.000004 4.15052 0 17.533521 12.613728 16.398735 0 0 0 0 0 0
$definebone "bip_index_1_R" "bip_index_0_R" -0.000001 1.686291 0 28.826482 34.326342 38.376111 0 0 0 0 0 0
$definebone "bip_index_2_R" "bip_index_1_R" -0.000001 1.280437 -0.000001 -66.403885 -126.812859 -31.026018 0 0 0 0 0 0
$definebone "middle_metacarpal_r" "bip_hand_R" 0.490405 1.008774 -0.181958 -43.59302 8.695994 -38.403029 0 0 0 0 0 0
$definebone "bip_middle_0_R" "middle_metacarpal_r" 0.000001 3.665703 0 7.589903 2.196359 5.98888 0 0 0 0 0 0
$definebone "bip_middle_1_R" "bip_middle_0_R" 0.000001 2.008636 0 31.073068 43.05977 50.044933 0 0 0 0 0 0
$definebone "bip_middle_2_R" "bip_middle_1_R" 0.000001 1.30563 0.000001 -74.899056 -106.981097 -57.270696 0 0 0 0 0 0
$definebone "pinky_metacarpal_r" "bip_hand_R" -1.222392 0.857635 -1.281573 -41.955762 18.661013 -46.94471 0 0 0 0 0 0
$definebone "bip_pinky_0_R" "pinky_metacarpal_r" 0 3.593906 0.000004 24.915304 3.229846 21.295094 0 0 0 0 0 0
$definebone "bip_pinky_1_R" "bip_pinky_0_R" 0 1.41539 -0.000001 32.075013 32.77219 40.36759 0 0 0 0 0 0
$definebone "bip_pinky_2_R" "bip_pinky_1_R" -0.000002 1.102364 0.000002 -72.80173 -131.607802 -35.065935 0 0 0 0 0 0
$definebone "ring_metacarpal_r" "bip_hand_R" -0.25193 0.98164 -0.829941 -39.435071 12.752839 -39.400018 0 0 0 0 0 0
$definebone "bip_ring_0_R" "ring_metacarpal_r" 0 3.529209 0.000002 12.577889 -1.923451 9.275796 0 0 0 0 0 0
$definebone "bip_ring_1_R" "bip_ring_0_R" 0 1.824162 -0.000002 34.061611 36.433134 46.380502 0 0 0 0 0 0
$definebone "bip_ring_2_R" "bip_ring_1_R" 0.000001 1.209011 0.000002 -78.084697 -105.383044 -62.546911 0 0 0 0 0 0
$definebone "bip_thumb_0_R" "bip_hand_R" 0.956855 0.749916 0.699585 27.259138 5.405673 25.008745 0 0 0 0 0 0
$definebone "bip_thumb_1_R" "bip_thumb_0_R" 0 2.966928 0 -17.05083 11.929662 -25.756847 0 0 0 0 0 0
$definebone "bip_thumb_2_R" "bip_thumb_1_R" 0 1.127216 0.000002 -57.014998 18.812419 162.63324 0 0 0 0 0 0
$definebone "lowerarm_twist_01_r" "bip_lowerArm_R" 0.000002 8.927464 -0.000004 36.972122 -0.000041 179.999938 0 0 0 0 0 0
$definebone "lowerarm_twist_02_r" "bip_lowerArm_R" 0.000002 4.809948 0 36.972122 -0.000032 179.999966 0 0 0 0 0 0
$definebone "upperarm_twist_01_r" "bip_upperArm_R" 0.000002 0.002052 0.000002 42.505722 -0.000001 180.000007 0 0 0 0 0 0
$definebone "upperarm_twist_02_r" "bip_upperArm_R" 0 6.897076 0 42.498848 -0.000027 179.999966 0 0 0 0 0 0
$definebone "bip_hip_L" "bip_pelvis" 0.492413 -1.18655 3.875037 -6.480087 21.270077 173.402385 0 0 0 0 0 0
$definebone "bip_knee_L" "bip_hip_L" -0.000001 19.524225 0 -1.954768 -4.343775 2.646839 0 0 0 0 0 0
$definebone "calf_twist_01_l" "bip_knee_L" 0.038377 17.90642 -0.071015 -10.320268 -0.237548 178.62845 0 0 0 0 0 0
$definebone "calf_twist_02_l" "bip_knee_L" 0.042287 9.4032 -0.026054 -10.334493 -0.822405 178.785285 0 0 0 0 0 0
$definebone "bip_foot_L" "bip_knee_L" -0.080666 21.28129 0.097071 3.802179 69.778798 -4.350289 0 0 0 0 0 0
$definebone "bip_toe_L" "bip_foot_L" 0 7.575901 -0.000001 19.673114 -154.17222 -172.466472 0 0 0 0 0 0
$definebone "thigh_twist_01_l" "bip_hip_L" -0.000001 10.815144 0 -13.11053 -0.000008 180.000007 0 0 0 0 0 0
$definebone "bip_hip_R" "bip_pelvis" 0.492359 -1.186493 -3.875027 6.481094 21.271446 -173.402536 0 0 0 0 0 0
$definebone "bip_knee_R" "bip_hip_R" 0.000001 19.524319 -0.000001 1.954784 -4.343763 -2.646844 0 0 0 0 0 0
$definebone "calf_twist_01_r" "bip_knee_R" 0.038377 17.906479 0.071018 -1.260368 -0.518828 1.348283 0 0 0 0 0 0
$definebone "calf_twist_02_r" "bip_knee_R" 0.042287 9.403233 0.026056 -1.260367 -0.518827 1.348282 0 0 0 0 0 0
$definebone "bip_foot_R" "bip_knee_R" -0.080662 21.281357 -0.097075 -3.802196 69.778832 4.350193 0 0 0 0 0 0
$definebone "bip_toe_R" "bip_foot_R" 0 7.575878 0.000001 9.35032 24.452771 -172.812475 0 0 0 0 0 0
$definebone "thigh_twist_01_r" "bip_hip_R" 0.000001 10.81514 -0.000001 0.000009 -0.000001 -0.000004 0 0 0 0 0 0


$sequence "idle" {
	"fortnite_br_reaper_anims\idle.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}
