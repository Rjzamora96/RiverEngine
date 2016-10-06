#include "Component.h"

#include <stdio.h>

namespace RiverEngine
{
	bool Component::s_breakable;
	luabridge::lua_State* Component::L;

	Component::Component() : updateFunc(nullptr)
	{
		owner = 0;
	}


	Component::~Component()
	{
	}

	void Component::SetName(const char * const name)
	{
		for (int j = 0; j < MAX_NAME_LEN; ++j)
		{
			m_name[j] = name[j];
			if (!name[j]) return;
		}
		m_name[-1] = 0;
	}

	bool Component::Init()
	{
		using namespace luabridge;
		if (luaL_dofile(L, m_scriptPath.c_str()) == 0)
		{
			LuaRef table = getGlobal(L, m_name);
			if (table.isTable())
			{
				if (table["Update"].isFunction())
				{
					updateFunc = std::make_shared<LuaRef>(table["Update"]);
				}
				else
				{
					updateFunc.reset();
				}
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