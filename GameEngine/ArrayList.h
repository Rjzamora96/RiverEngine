#pragma once

namespace RiverEngine
{
	template <class T>
	class ArrayList
	{
	public:
		ArrayList(int capacity = 0);
		~ArrayList();
		void Add(T t);
		bool Empty();
		int Count();
		T operator[](int i);
	private:
		T* m_array;
		int m_count;
		int m_size;
	};

	template <class T>
	ArrayList<T>::ArrayList(int capacity) : m_count(0), m_size(capacity)
	{
		m_array = new T[m_size];
	}

	template <class T>
	ArrayList<T>::~ArrayList()
	{
		delete[] m_array;
	}

	template <class T>
	void ArrayList<T>::Add(T t)
	{
		if (m_count == m_size - 1)
		{
			T* temp = new T[m_size * 2]
				for (int i = 0; i < m_size; i++)
				{
					temp[i] = m_array[i];
				}
			m_size *= 2;
			delete[] m_array;
			m_array = temp;
		}
		m_array[m_count] = t;
		m_count++;
	}

	template <class T>
	bool ArrayList<T>::Empty()
	{
		return m_count == 0;
	}

	template <class T>
	int ArrayList<T>::Count()
	{
		return m_count;
	}
	template<class T>
	T ArrayList<T>::operator[](int i)
	{
		return m_array[i];
	}
}