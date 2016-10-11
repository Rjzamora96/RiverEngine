#pragma once

#include "ArrayList.h"
#include "Renderable.h"
#include "Sprite.h"
#include <unordered_map>
#include "Texture.h"

class RenderableManager
{
public:
	RenderableManager();
	~RenderableManager();
	static void AddSprite(RiverEngine::Sprite* sprite, std::string path);
	static void ChangeSprite(RiverEngine::Sprite* sprite, std::string path);
	static void AddTexture(std::string path);
	static void Initialize();
	static std::unordered_map<std::string, Texture*> spriteMap;
	static RiverEngine::ArrayList<Renderable*> renderables;
private:
	static unsigned int m_spriteCount;
};

