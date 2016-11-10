#include "Scene.h"

#include "LuaBridge.h"

namespace RiverEngine
{
	Scene* Scene::m_activeScene = 0;
	luabridge::lua_State* Scene::L = 0;
	bool Scene::m_toChangeScene = false;
	ArrayList<Entity*> Scene::m_instantiateQueue = ArrayList<Entity*>();
	std::string Scene::m_newScenePath = "";
	ClearRenderables Scene::onSceneChange;

	Scene::Scene()
	{
	}

	Scene::~Scene()
	{
		for (int i = 0; i < m_entityList.Count(); i++)
		{
			delete m_entityList[i];
		}
	}

	Scene * Scene::GetActiveScene() { return m_activeScene; }

	void Scene::SetActiveScene(Scene * m) { m_activeScene = m; }

	void Scene::Update(float dt)
	{
		for (int i = 0; i < m_activeScene->m_entityList.Count(); i++)
		{
			m_activeScene->m_entityList[i]->Update(dt);
		}
		if (m_toChangeScene)
		{
			if (onSceneChange != 0) onSceneChange();
			delete m_activeScene;
			m_activeScene = new Scene();
			m_activeScene->LoadSceneFromFile(m_newScenePath);
			m_toChangeScene = false;
		}
		for (int i = 0; i < m_instantiateQueue.Count(); i++)
		{
			m_activeScene->m_entityList.Add(m_instantiateQueue[i]);
		}
		m_instantiateQueue.Clear();
	}

	void Scene::AddEntity(Entity * e)
	{
		m_activeScene->m_entityList.Add(e);
		for (int i = 0; i < e->children.Count(); i++)
		{
			m_activeScene->AddEntity(e->children[i]);
		}
	}

	Entity * Scene::GetEntityByTag(std::string tag)
	{
		for (int i = 0; i < m_activeScene->m_entityList.Count(); i++)
		{
			if (m_activeScene->m_entityList[i]->HasTag(tag)) return m_activeScene->m_entityList[i];
			else
			{
				Entity* result = m_activeScene->m_entityList[i]->GetChildByTag(tag);
				if (result != 0) return result;
			}
		}
		return 0;
	}
	luabridge::LuaRef Scene::GetEntities()
	{
		luabridge::LuaRef entityArray = luabridge::LuaRef::newTable(L);
		for (int i = 0; i < m_activeScene->m_entityList.Count(); i++)
		{
			entityArray[i+1] = m_activeScene->m_entityList[i];
		}
		return entityArray;
	}
	void Scene::ChangeScene(std::string path)
	{
		m_toChangeScene = true;
		m_newScenePath = path;
	}
	void Scene::Instantiate(std::string path)
	{
		Entity* entity = new Entity();
		entity->LoadComponents(path);
		m_instantiateQueue.Add(entity);
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