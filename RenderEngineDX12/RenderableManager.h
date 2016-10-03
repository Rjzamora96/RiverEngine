#pragma once

#include "ArrayList.h"
#include "Renderable.h"

class RenderableManager
{
public:
	RenderableManager();
	~RenderableManager();
	void AddSprite(wchar_t* path);
private:
	RiverEngine::ArrayList<Renderable> m_renderables;
};

