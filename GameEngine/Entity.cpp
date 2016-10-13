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
		transform = new Transform();
		AddComponent(transform, "Transform");
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