#pragma once

#include "ArrayList.h"
#include "Renderable.h"
#include "Sprite.h"
#include <unordered_map>

class RenderableManager
{
public:
	RenderableManager();
	~RenderableManager();
	static void AddSprite(RiverEngine::Sprite* sprite, std::string path);
	static void ChangeSprite(RiverEngine::Sprite* sprite, std::string path);
	static void Initialize();
	static std::unordered_map<std::string, Microsoft::WRL::ComPtr<ID3D12Resource>> spriteMap;
	static RiverEngine::ArrayList<Renderable> renderables;
};

