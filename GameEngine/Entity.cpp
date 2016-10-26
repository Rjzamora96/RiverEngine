#include "Entity.h"

#include <string.h>
#include "Component.h"
#include "Transform.h"
#include "Sprite.h"
#include "LuaBridge.h"
namespace RiverEngine
{
	luabridge::lua_State* Entity::L;
	Entity::Entity()
	{
		memset(m_components, 0, MAX_COMPONENTS * sizeof(m_components[0]));
	}


	Entity::~Entity()
	{
	}

	void Entity::SetName(const char * const name)
	{
		for (int j = 0; j < MAX_NAME_LEN; ++j)
		{
			m_name[j] = name[j];
			if (!name[j]) return;
		}
		m_name[MAX_NAME_LEN - 1] = 0;
	}

	bool Entity::AddComponent(Component * c, const char * const name)
	{
		for (int j = 0; j < MAX_COMPONENTS; ++j)
		{
			if (!m_components[j])
			{
				if (typeid(*c) == typeid(Sprite)) sprite = static_cast<Sprite*>(c);
				if (typeid(*c) == typeid(Transform)) transform = static_cast<Transform*>(c);
				m_components[j] = c;
				c->SetOwner(this);
				c->SetName(name);
				break;
			}
		}
		return true;
	}

	void Entity::SendMessage(std::string id, luabridge::LuaRef message, luabridge::LuaRef sender)
	{
		for (int i = 0; i < MAX_COMPONENTS; i++)
		{
			if(m_components[i]) m_components[i]->OnMessage(id, message, sender);
		}
	}

	luabridge::LuaRef Entity::GetComponent(std::string name)
	{
		if (name.compare("Sprite") == 0) return luabridge::LuaRef(L, sprite);
		for (int i = 0; i < MAX_COMPONENTS; i++)
		{
			if (name.compare(m_components[i]->GetName()) == 0) return m_components[i]->GetProperties();
		}
		return luabridge::LuaRef(L, nullptr);
	}

	void Entity::LoadComponents(std::string script)
	{
		using namespace luabridge;
		if (luaL_dofile(L, script.c_str()) == 0)
		{
			LuaRef table = getGlobal(L, "entity");
			int tableLen = table.length();
			for (int i = 1; i <= table.length(); ++i)
			{
				Component* c = new Component();
				LuaRef subTable = table[i];
				if (subTable["script"].isString())
				{
					c->SetScript(subTable["script"]);
				}
				if (subTable["componentName"].isString())
				{
					if (subTable["componentName"].cast<std::string>().compare("transform") == 0)
					{
						delete c;
						c = new Transform();
						LuaRef position = subTable["position"];
						float x = position[1].cast<float>();
						//bool test = subTable["position"][0].isNumber();
						((Transform*)c)->position->x = position[1].cast<float>();
						((Transform*)c)->position->y = position[2].cast<float>();
						((Transform*)c)->rotation = subTable["rotation"].cast<float>();
						((Transform*)c)->scale = subTable["scale"].cast<float>();
						transform = static_cast<Transform*>(c);
						AddComponent(c, "transform");
					}
					else AddComponent(c, subTable["componentName"]);
				}
				c->Init();
				ArrayList<std::string> keyList = c->GetKeyList();
				LuaRef properties = c->GetProperties();
				for (int j = 0; j < keyList.Count(); ++j)
				{
					std::string key = keyList[j];
					if (!subTable[key].isNil())
					{
						LuaRef value = subTable[key];
						properties[key] = value;
					}
					//subTable[key];
				}
			}
		}
	}

	bool Entity::Update(float dt)
	{
		for (int j = 0; j < MAX_COMPONENTS; ++j)
		{
			if (m_components[j] && m_components[j]->IsEnabled())
			{
				bool result = m_components[j]->Update(dt);
			}
		}
		return true;
	}

	bool Entity::Initialize()
	{
		for (int j = 0; j < MAX_COMPONENTS; ++j)
		{
			if (m_components[j])
			{
				if (!m_components[j]->Init()) return false;
			}
		}
		return true;
	}
}