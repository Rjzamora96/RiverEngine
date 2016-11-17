#include "Entity.h"

#include <string.h>
#include "Component.h"
#include "Transform.h"
#include "Sprite.h"

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

	void Entity::SetName(const std::string const name)
	{
		m_name = name;
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
			LoadComponentsFromTable(table);
		}
	}

	void Entity::LoadComponentsFromTable(luabridge::LuaRef table)
	{
		using namespace luabridge;
		LuaRef entityName = table["name"];
		LuaRef tagList = table["tags"];
		if (entityName.isString()) m_name = entityName.cast<std::string>();
		if (tagList.isTable()) for (int i = 1; i <= tagList.length(); ++i) if (tagList[i].isString()) AddTag(tagList[i].cast<std::string>());
		LuaRef compTable = table["components"];
		for (int i = 1; i <= compTable.length(); ++i)
		{
			Component* c = new Component();
			LuaRef subTable = compTable[i];
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
					((Transform*)c)->localPosition->x = position[1].cast<float>();
					((Transform*)c)->localPosition->y = position[2].cast<float>();
					((Transform*)c)->localRotation = subTable["rotation"].cast<float>();
					((Transform*)c)->localScale = subTable["scale"].cast<float>();
					AddComponent(c, "transform");
				}
				else if (subTable["componentName"].cast<std::string>().compare("sprite") == 0)
				{
					delete c;
					c = new Sprite();
					LuaRef origin = subTable["origin"];
					((Sprite*)c)->origin.x = origin[1].cast<float>();
					((Sprite*)c)->origin.y = origin[2].cast<float>();
					LuaRef rect = subTable["rectangle"];
					if (!rect.isNil())
					{
						((Sprite*)c)->rect.x = rect[1].cast<float>();
						((Sprite*)c)->rect.y = rect[2].cast<float>();
						((Sprite*)c)->rect.width = rect[3].cast<float>();
						((Sprite*)c)->rect.height = rect[4].cast<float>();
						((Sprite*)c)->usesRect = true;
					}
					AddComponent(c, "sprite");
					Sprite::addSprite((Sprite*)c, subTable["sprite"].cast<std::string>());
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
		LuaRef childrenTable = table["children"];
		for (int i = 1; i <= childrenTable.length(); i++)
		{
			Entity* child = new Entity();
			child->LoadComponentsFromTable(childrenTable[i]);
			children.Add(child);
			child->parent = this;
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