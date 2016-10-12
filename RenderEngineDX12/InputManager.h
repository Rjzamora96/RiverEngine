#pragma once

#include "Keyboard.h"

class InputManager
{
public:
	static void Initialize();
	static bool CheckKeyDown(int key);
private:
	static std::unique_ptr<DirectX::Keyboard> m_keyboard;
};

