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
		void SetName(const char* const name);
		void SetOwner(Entity* owner) { this->owner = owner; }
		void SetScript(const std::string& script) { m_scriptPath = script; }
		bool Init();
		virtual bool Update(float dt) { if (updateFunc) (*updateFunc)(this, dt); return true; }
		virtual bool Draw() { return true; }
		virtual bool Initialize() { return true; }
		void Enable(bool enabled = true) { m_enabled = enabled; }
		bool IsEnabled() const { return m_enabled; }
		bool IsDisabled() const { return !m_enabled; }
		static void SetBreak(bool enabled = true);
		static void ToggleBreak() { s_breakable = !s_breakable; }
		static void Break(bool condition = true, bool keepBreakable = true);
		static void BreakIf(bool condition = true);
		Entity* owner;
	protected:
		template <class T> T* GetSiblingComponent() { return owner->GetComponentByType<T>(); }
		char m_name[MAX_NAME_LEN];
		bool m_enabled = true;
		std::shared_ptr<luabridge::LuaRef> updateFunc;
	private:
		static bool s_breakable;
		static luabridge::lua_State* L;
		std::string m_scriptPath;
	};


}