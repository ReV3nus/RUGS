import os
import json
from argparse import ArgumentParser
from typing import Dict, Optional

import imageio.v2 as imageio
import numpy as np
import torch
import torch.nn.functional as F
import torchvision
from tqdm import tqdm
from PIL import Image
from lpips import LPIPS

from arguments import GroupParams, ModelParams, PipelineParams, get_combined_args
from gaussian_renderer import GaussianModel, render
from gs_ir import recon_occlusion, IrradianceVolumes
from pbr import CubemapLight, get_brdf_lut, pbr_shading
from scene import Scene
from utils.general_utils import safe_state
from utils.image_utils import viridis_cmap, psnr as get_psnr
from utils.loss_utils import ssim as get_ssim

from utils.system_utils import mkdir_p
import sys
import shutil

if len(sys.argv) < 2:
    print("Usage: python pth2ply.py [model_name]")
    sys.exit(1)

model_name = sys.argv[1]
model_path = "outputs/" + model_name
output_path = "outputs/" + model_name + "/" + model_name


gaussians = GaussianModel(3)
checkpoint = torch.load(model_path + "/chkpnt40000.pth")
model_params = checkpoint["gaussians"]
gaussians.restore(model_params)

mkdir_p(os.path.dirname(output_path + "/point_cloud/iteration_30000/"))
gaussians.save_ply(output_path + "/point_cloud/iteration_30000/point_cloud.ply")

shutil.copy(model_path + "/cfg_args", output_path + "/cfg_args")
shutil.copy(model_path + "/cameras.json", output_path + "/cameras.json")
shutil.copy(model_path + "/input.ply", output_path + "/input.ply")
