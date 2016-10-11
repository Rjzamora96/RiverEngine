#pragma once

#include "pch.h"

struct Texture
{
	unsigned int id;
	Microsoft::WRL::ComPtr<ID3D12Resource> texture;
};