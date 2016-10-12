#include "Component.h"

#include <stdio.h>

namespace RiverEngine
{
	bool Component::s_breakable;
	luabridge::lua_State* Component::L;
	Component* Component::activeComponent;

	Component::Component() : updateFunc(nullptr), m_scriptPath("")
	{
		owner = 0;
	}


	Component::~Component()
	{
	}

	bool Component::Init()
	{
		using namespace luabridge;
		Component::activeComponent = this;
		m_properties["entity"] = this->owner;
		if (luaL_dofile(L, m_scriptPath.c_str()) == 0)
		{
			LuaRef update = getGlobal(L, "Update");
			if (update.isFunction())
			{
				updateFunc = std::make_shared<LuaRef>(update);
			}
			else
			{
				updateFunc.reset();
			}
		}
		m_enabled = true;
		bool result = Initialize();
		if (!result) printf("Component failed to initialize!\n");
		else printf("Component Initialized successfully!\n");
		return true;
	}

	void Component::SetBreak(bool enabled)
	{
		s_breakable = enabled;
	}

	void Component::Break(bool condition, bool keepBreakable)
	{
		if (condition && s_breakable)
		{
			__debugbreak();
			s_breakable = keepBreakable;
		}
	}

	void Component::BreakIf(bool condition)
	{
		if (condition)
		{
			__debugbreak();
		}
	}
}