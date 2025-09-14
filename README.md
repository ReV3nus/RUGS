# RUGS: Relightable Unity Gaussian Splatting

This project is based on [3DGS](https://github.com/graphdeco-inria/gaussian-splatting), [GS-IR](https://github.com/lzhnb/GS-IR) and [UnityGaussianSplatting](https://github.com/aras-p/UnityGaussianSplatting).

## Introduction

**3D reconstruction**, originating from computer graphics, aims to recover informations of 3Dscenes from 2D images or sensor data and holds significant value in many fields. Among current 3D scene reconstruction methods, **3D Gaussian Splatting (3DGS)** has emerged as a novel scene representation and rendering approach by leveraging learnable 3D Gaussian distributions and thereby achieves high-quality real-time rendering with remarkable computational efficiency. 

Additionally, **relighting** technology, which aims to modify illumination conditions while preserving scene geometry, has important value of enhancing realism and immersion in related fields as well. Building upon existing techniques, this paper implements and refines a complete pipeline for 3DGS-based scene reconstruction and relighting. 

 - Firstly, the pipeline reconstructs 3D scenes and performs inverse rendering using 2D image data based on **3DGS** and its inverse rendering extension **GS-IR(3D Gaussian Splatting for Inverse Rendering)**, recovering scene appearance and furthermore physical material properties.
 - Subsequently, this paper introduces **RUGS (Relightable Unity Gaussian Splatting)**—a new real-time relighting and rendering framework developed in Unity, based on the open-source 3DGS renderer UnityGaussianSplatting. After converting and processing the extended point cloud data, RUGS utilizes a custom rendering pipeline and compute shaders to achieve image-based lighting (IBL) for fast and realistic real-time rendering.
 - Additionally, RUGS supports hybrid rendering with virtual objects and light sources, and provides curve-based scene editing features to further enhance practicality and demonstrate extensibility.

Through these open-source frameworks and methodologies, this paper demonstrates a comprehensive 3D scene reconstruction and relighting pipeline, along with high-quality real-time rendering results. With RUGS’s interactive and extensible framework, the proposed system provides a valuable foundation and reference for future research and applications in related fields.

In addition, this project also implements a sampling logic in Unity, which is able to capture RGBD data with a [sampler](https://github.com/ReV3nus/UnityRGBDepthSceneSampler) for both BiRP and HDRP Unity scenes and converts these data to a input dataset. These datasets can be used for **GS-IR** after running _convert.py_ from **3DGS**.

## Instructions for use

To use **RUGS** in Unity, you need to prepare the model file trained by **GS-IR** which should be a .pth file. You can get this file using the instruction of **GS-IR**. And if you want to train datasets of a virtual scene, you can refer to the sampler aforementioned and try to use the sampler in `/utils/UnitySample`.

After you get your .pth file, you should use the python script in `/utils/pth2ply.py` to convert it into .ply file by putting it into the root folder of **GS-IR** and running: 
`python pth2ply.py [model_name]`

Finally, you can import your .pth file into the **RUGS** open `Tools -> Gaussian Splats -> Create GaussianSplatAsset` menu within Unity and modify its properties in Inspector to get the desired rendering effect.

<figure>
<p align="center" width="100%">
  <img src="https://github.com/user-attachments/assets/186dfcc7-c838-4d65-bd09-d4d626869094" alt="image" style="width:60%">
  <p align="center" width="100%"><text align="text:center">Configuration</text></p>
  </p>
</figure>

## Experiments and Results

<p align="center" width="100%"><h4 align="center">Relighting with different environment maps</h4></p>
<figure>
<p align="center" width="100%">
  <img src="https://github.com/user-attachments/assets/7481347b-bb7a-4bb4-9e78-a08dcb23228c" alt="image" style="width:80%">
  </p>
</figure>

<p align="center" width="100%"><h4 align="center">Different point-light properties</h4></p>
<figure>
<p align="center" width="100%">
  <img src="https://github.com/user-attachments/assets/36bc4a21-fe78-4fb1-b24f-ab110e2ec1f0" alt="image" style="width:80%">
  </p>
</figure>

<p align="center" width="100%"><h4 align="center">Comparation of relighting between GS-IR and RUGS</h4></p>
<figure>
<p align="center" width="100%">
  <img src="https://github.com/user-attachments/assets/534bad8f-c7c8-4c24-a148-887ed22151f1" alt="image" style="width:80%">
  </p>
</figure>

<p align="center" width="100%"><h4 align="center">Curves</h4></p>
<figure>
<p align="center" width="100%">
  <img src="https://github.com/user-attachments/assets/a83f7a48-9b32-44c5-80f9-1acd374764d6" alt="image" style="width:80%">
  </p>
</figure>

<p align="center" width="100%"><h4 align="center">My dormitory</h4></p>
<figure>
<p align="center" width="100%">
  <img src="https://github.com/user-attachments/assets/11d4646a-40aa-47a7-8c8a-9955e2d2403f" alt="image" style="width:80%">
  </p>
</figure>

For the specific implementation process, please refer to the [graduation thesis](https://github.com/ReV3nus/RUGS/blob/main/%E5%8F%AF%E9%87%8D%E5%85%89%E7%85%A73D%E9%AB%98%E6%96%AF%E9%87%8D%E5%BB%BA%E4%B8%8E%E6%B8%B2%E6%9F%93-%E9%92%B1%E9%9F%A6%E5%85%8B.pdf).

