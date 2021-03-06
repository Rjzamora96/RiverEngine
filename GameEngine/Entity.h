#pragma once

#include "LuaBridge.h"
#include "ArrayList.h"

namespace RiverEngine
{
	class Component;
	class Transform;
	class Sprite;

	class Entity
	{
		enum
		{
			MAX_COMPONENTS = 10,
			MAX_NAME_LEN = 30
		};
	public:
		Entity();
		~Entity();
		static void AssignState(luabridge::lua_State* l) { L = l; }
		void SetName(const std::string const name);
		const std::string GetName() const { return m_name; }
		bool AddComponent(Component* c, const char* const name);
		void AddTag(std::string tag) { m_tags.Add(tag); }
		bool HasTag(std::string tag) { for (int i = 0; i < m_tags.Count(); i++) if (m_tags[i].compare(tag) == 0) return true; return false; }
		Entity* GetChildByTag(std::string tag)
		{
			for (int i = 0; i < children.Count(); i++)
			{
				if (children[i]->HasTag(tag)) return children[i];
				else
				{
					Entity* result = children[i]->GetChildByTag(tag);
					if (result != 0) return result;
				}
			}
			return 0;
		}
		void SendMessage(std::string id, luabridge::LuaRef message, luabridge::LuaRef sender);
		template <class T> T* GetComponentByType() const;
		luabridge::LuaRef GetComponent(std::string name);
		void LoadComponents(std::string script);
		void LoadComponentsFromTable(luabridge::LuaRef table);
		Entity* parent;
		ArrayList<Entity*> children;
		Transform* transform;
		Sprite* sprite;
		bool Update(float dt);
	protected:
		bool Initialize();
	private:
		std::string m_name;
		ArrayList<std::string> m_tags;
		Component* m_components[MAX_COMPONENTS];
		static luabridge::lua_State* L;
	};

	template<class T>
	inline T * Entity::GetComponentByType() const
	{
		for (int j = 0; j < MAX_COMPONENTS; ++j)
		{
			if (!m_components[j]) continue;
			if (typeid(T) == typeid(*m_components[j]))
			{
				if (m_components[j]->IsDisabled()) return 0;
				return static_cast<T*>(m_components[j]);
			}
		}
		return 0;
	}
}