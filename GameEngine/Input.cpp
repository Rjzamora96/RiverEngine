#include "Input.h"

namespace RiverEngine
{
	std::unordered_map<std::string, Input::Keys> Input::keyMap;
	BindPtr Input::keyDown;

	void Input::InitializeBindings()
	{
		keyMap["Up"] = Keys::Up;
		keyMap["Down"] = Keys::Down;
		keyMap["Left"] = Keys::Left;
		keyMap["Right"] = Keys::Right;
	}

	bool Input::IsKeyDown(std::string key)
	{
		return keyDown(keyMap[key]);
	}

}