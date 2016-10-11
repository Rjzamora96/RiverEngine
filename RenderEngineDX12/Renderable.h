#pragma once

#include "pch.h"
#include "Sprite.h"
#include "Texture.h"

struct Renderable
{
	RiverEngine::Sprite* sprite;
	Texture* texture;
	float layer;
	unsigned int id;
};

