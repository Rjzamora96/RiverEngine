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
		static void AssignState(luabridge::lua_State* l) { L = l; }
		static void SetActiveScene(Scene* m);
		static void Update(float dt);
		static void AddEntity(Entity* e);
		static Entity* GetEntityByTag(std::string tag);
		void LoadSceneFromFile(std::string path);
	private:
		static Scene* m_activeScene;
		static luabridge::lua_State* L;
		ArrayList<Entity*> m_entityList;
	};
}