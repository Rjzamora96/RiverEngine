#pragma once

#include "Entity.h"
#include <memory>
#include "LuaBridge.h"
#include <string>
namespace RiverEngine
{
	class lua_State;

	class Component
	{
		enum
		{
			MAX_COMPONENTS = 10,
			MAX_NAME_LEN = 30
		};
	public:
		Component();
		~Component();
		static void AssignState(luabridge::lua_State* l) { L = l; }
		void SetName(std::string name) { m_name = name; }
		std::string GetName() { return m_name; }
		luabridge::LuaRef GetProperties() { return m_properties; }
		void SetOwner(Entity* owner) { this->owner = owner; }
		void SetScript(const std::string& script) { m_scriptPath = script; }
		bool Init();
		virtual bool Update(float dt) { if (updateFunc) (*updateFunc)(m_properties, dt); return true; }
		virtual bool Draw() { return true; }
		virtual bool Initialize() { if (initializeFunc) (*initializeFunc)(m_properties); return true; }
		virtual void OnMessage(std::string id, luabridge::LuaRef message, luabridge::LuaRef sender) { if (messageFunc) (*messageFunc)(m_properties, id, message, sender); }
		void Enable(bool enabled = true) { m_enabled = enabled; }
		bool IsEnabled() const { return m_enabled; }
		bool IsDisabled() const { return !m_enabled; }
		static void Property(const std::string& name, luabridge::LuaRef value) { activeComponent->m_properties[name] = value; }
		static void SetBreak(bool enabled = true);
		static void ToggleBreak() { s_breakable = !s_breakable; }
		static void Break(bool condition = true, bool keepBreakable = true);
		static void BreakIf(bool condition = true);
		Entity* owner;
	protected:
		template <class T> T* GetSiblingComponent() { return owner->GetComponentByType<T>(); }
		std::string m_name;
		bool m_enabled = true;
		std::shared_ptr<luabridge::LuaRef> initializeFunc;
		std::shared_ptr<luabridge::LuaRef> updateFunc;
		std::shared_ptr<luabridge::LuaRef> messageFunc;
	private:
		static bool s_breakable;
		static luabridge::lua_State* L;
		static Component* activeComponent;
		luabridge::LuaRef m_properties = luabridge::newTable(L);
		std::string m_scriptPath;
	};


}