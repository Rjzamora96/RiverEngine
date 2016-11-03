#include "pch.h"
#include "RenderableManager.h"
#include "Sprite.h"
#include "Transform.h"
#include "Scene.h"

RiverEngine::ArrayList<Renderable*> RenderableManager::renderables = RiverEngine::ArrayList<Renderable*>();
std::unordered_map<std::string, Texture*> RenderableManager::spriteMap;
unsigned int RenderableManager::spriteCount = 0;

RenderableManager::RenderableManager()
{
}

RenderableManager::~RenderableManager()
{
}

void RenderableManager::AddSprite(RiverEngine::Sprite* sprite, std::string path)
{
	Texture* existingTexture = spriteMap[path];
	if (existingTexture == 0)
	{
		Texture* t = new Texture();
		t->id = spriteCount;
		t->texture = Microsoft::WRL::ComPtr<ID3D12Resource>();
		spriteMap[path] = t;
		spriteCount++;
	}
	Renderable* r = new Renderable();
	r->id = renderables.Count();
	sprite->id = r->id;
	r->sprite = sprite;
	r->layer = 0.0f;
	r->texture = spriteMap[path];
	r->transform = sprite->owner->transform;
	renderables.Add(r);
}

void RenderableManager::ChangeSprite(RiverEngine::Sprite* sprite, std::string path)
{
	for (int i = 0; i < renderables.Count(); i++)
	{
		if (renderables[i]->id == sprite->id)
		{
			renderables[i]->texture = spriteMap[path];
		}
	}
}

void RenderableManager::AddTexture(std::string path)
{
	Texture* existingTexture = spriteMap[path];
	if (existingTexture == 0)
	{
		Texture* t = new Texture();
		t->id = spriteCount;
		t->texture = Microsoft::WRL::ComPtr<ID3D12Resource>();
		spriteMap[path] = t;
		spriteCount++;
	}
}

void RenderableManager::ClearRenderables()
{
	renderables.Clear();
}

void RenderableManager::Initialize()
{
	RiverEngine::Sprite::addSprite = &RenderableManager::AddSprite;
	RiverEngine::Sprite::changeSprite = &RenderableManager::ChangeSprite;
	RiverEngine::Sprite::addTexture = &RenderableManager::AddTexture;
	RiverEngine::Scene::onSceneChange = &RenderableManager::ClearRenderables;
}