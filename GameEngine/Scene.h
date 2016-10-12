#pragma once

#include "ArrayList.h"
#include <string>
#include "Entity.h"

namespace RiverEngine
{
	class Scene
	{
	public:
		Scene();
		~Scene();
		static Scene* GetActiveScene();
		static void SetActiveScene(Scene* m);
		static void Update(float dt);
		static void AddEntity(Entity* e);
		static Entity* GetEntityByTag(std::string tag);
	private:
		static Scene* m_activeScene;
		ArrayList<Entity*> m_entityList;
	};
}