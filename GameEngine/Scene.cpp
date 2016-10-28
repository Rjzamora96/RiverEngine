#include "Scene.h"

#include "LuaBridge.h"

namespace RiverEngine
{
	Scene* Scene::m_activeScene = 0;
	luabridge::lua_State* Scene::L = 0;

	Scene::Scene()
	{
	}

	Scene::~Scene()
	{
	}

	Scene * Scene::GetActiveScene() { return m_activeScene; }

	void Scene::SetActiveScene(Scene * m) { m_activeScene = m; }

	void Scene::Update(float dt)
	{
		for (int i = 0; i < m_activeScene->m_entityList.Count(); i++)
		{
			m_activeScene->m_entityList[i]->Update(dt);
		}
	}

	void Scene::AddEntity(Entity * e)
	{
		m_activeScene->m_entityList.Add(e);
	}

	Entity * Scene::GetEntityByTag(std::string tag)
	{
		for (int i = 0; i < m_activeScene->m_entityList.Count(); i++)
		{
			if (m_activeScene->m_entityList[i]->HasTag(tag)) return m_activeScene->m_entityList[i];
		}
		return 0;
	}
	void Scene::LoadSceneFromFile(std::string path)
	{
		using namespace luabridge;
		if (luaL_dofile(L, path.c_str()) == 0)
		{
			LuaRef table = getGlobal(L, "scene");
			for (int i = 1; i <= table.length(); i++)
			{
				Entity* entity = new Entity();
				entity->LoadComponentsFromTable(table[i]);
				AddEntity(entity);
			}
		}
	}
}