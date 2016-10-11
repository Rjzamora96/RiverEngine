#pragma once

#include "pch.h"
#include "Sprite.h"

struct Renderable
{
	RiverEngine::Sprite* sprite;
	Microsoft::WRL::ComPtr<ID3D12Resource>* texture;
	float layer;
	unsigned int id;
};

