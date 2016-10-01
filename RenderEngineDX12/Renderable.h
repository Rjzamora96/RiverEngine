#pragma once

#include "pch.h"

struct Renderable
{
	DirectX::SimpleMath::Vector2 origin;
	DirectX::SimpleMath::Vector2 position;
	Microsoft::WRL::ComPtr<ID3D12Resource> texture;
	float layer;
	unsigned id;
};

