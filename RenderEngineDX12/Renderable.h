#pragma once

#include "pch.h"
#include "Sprite.h"
#include "Texture.h"
#include "Transform.h"

struct Renderable
{
	RiverEngine::Sprite* sprite;
	Texture* texture;
	RiverEngine::Transform* transform;
	float layer;
	unsigned int id;
};

