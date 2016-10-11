#include "pch.h"
#include "RenderableManager.h"
#include "Sprite.h"

RiverEngine::ArrayList<Renderable> RenderableManager::renderables = RiverEngine::ArrayList<Renderable>();

RenderableManager::RenderableManager()
{
}

RenderableManager::~RenderableManager()
{
}

void RenderableManager::AddSprite(RiverEngine::Sprite* sprite, std::string path)
{
	Microsoft::WRL::ComPtr<ID3D12Resource> existingTexture = spriteMap[path];
	if (existingTexture == 0)
	{
		spriteMap[path] = Microsoft::WRL::ComPtr<ID3D12Resource>();
	}
	Renderable r;
	r.id = renderables.Count();
	sprite->id = r.id;
	r.sprite = sprite;
	r.layer = 0.0f;
	r.texture = &spriteMap[path];
	renderables.Add(r);
}

void RenderableManager::ChangeSprite(RiverEngine::Sprite* sprite, std::string path)
{
	for (int i = 0; i < renderables.Count(); i++)
	{
		if (renderables[i].id == sprite->id)
		{
			renderables[i].texture = &spriteMap[path];
		}
	}
}

void RenderableManager::Initialize()
{
	RiverEngine::Sprite::addSprite = &RenderableManager::AddSprite;
	RiverEngine::Sprite::changeSprite = &RenderableManager::ChangeSprite;
}