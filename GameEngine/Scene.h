#pragma once

#include "ArrayList.h"
#include <string>
#include "Entity.h"

namespace RiverEngine
{
	typedef void(*ClearRenderables)();
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
		static luabridge::LuaRef GetEntities();
		static bool m_toChangeScene;
		static std::string m_newScenePath;
		static void ChangeScene(std::string path);
		static void Instantiate(std::string path);
		void LoadSceneFromFile(std::string path);
		static ClearRenderables onSceneChange;
	private:
		static Scene* m_activeScene;
		static luabridge::lua_State* L;
		static ArrayList<Entity*> m_instantiateQueue;
		ArrayList<Entity*> m_entityList;
	};
}