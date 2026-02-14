import { useState } from 'react'
import './App.css'

function App() {
  // Sample data with ID/Text pairs
  const [items] = useState([
    { id: 1, text: 'Implement user authentication system' },
    { id: 2, text: 'Design responsive dashboard layout' },
    { id: 3, text: 'Optimize database query performance' },
    { id: 4, text: 'Create API documentation' },
    { id: 5, text: 'Set up continuous integration pipeline' },
    { id: 6, text: 'Develop mobile application interface' },
    { id: 7, text: 'Implement real-time notifications' },
    { id: 8, text: 'Add multi-language support' },
  ])

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900">
      {/* Animated background elements */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-purple-500 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-blob"></div>
        <div className="absolute top-1/3 right-1/4 w-96 h-96 bg-blue-500 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-blob animation-delay-2000"></div>
        <div className="absolute bottom-1/4 left-1/3 w-96 h-96 bg-pink-500 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-blob animation-delay-4000"></div>
      </div>

      {/* Main content */}
      <div className="relative z-10 container mx-auto px-4 py-12">
        {/* Header */}
        <div className="text-center mb-12">
          <h1 className="text-5xl font-bold text-white mb-4 tracking-tight">
            <span className="bg-gradient-to-r from-blue-400 via-purple-400 to-pink-400 bg-clip-text text-transparent">
              Data Collection
            </span>
          </h1>
          <p className="text-gray-300 text-lg">Manage and view your ID/Text pairs</p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
          <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-6 border border-white/20 shadow-2xl hover:shadow-purple-500/20 transition-all duration-300 hover:scale-105">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-300 text-sm mb-1">Total Items</p>
                <p className="text-white text-3xl font-bold">{items.length}</p>
              </div>
              <div className="bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl p-3">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
            </div>
          </div>

          <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-6 border border-white/20 shadow-2xl hover:shadow-blue-500/20 transition-all duration-300 hover:scale-105">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-300 text-sm mb-1">Active</p>
                <p className="text-white text-3xl font-bold">{items.length}</p>
              </div>
              <div className="bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl p-3">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                </svg>
              </div>
            </div>
          </div>

          <div className="bg-white/10 backdrop-blur-lg rounded-2xl p-6 border border-white/20 shadow-2xl hover:shadow-pink-500/20 transition-all duration-300 hover:scale-105">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-300 text-sm mb-1">Last Updated</p>
                <p className="text-white text-xl font-bold">Just Now</p>
              </div>
              <div className="bg-gradient-to-br from-pink-500 to-rose-600 rounded-xl p-3">
                <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
            </div>
          </div>
        </div>

        {/* Items List */}
        <div className="bg-white/10 backdrop-blur-lg rounded-2xl border border-white/20 shadow-2xl overflow-hidden">
          <div className="px-6 py-4 border-b border-white/10 bg-white/5">
            <h2 className="text-2xl font-semibold text-white">Items List</h2>
          </div>
          
          <div className="divide-y divide-white/10">
            {items.map((item, index) => (
              <div
                key={item.id}
                className="px-6 py-5 hover:bg-white/5 transition-all duration-300 group cursor-pointer"
                style={{
                  animation: `fadeInUp 0.5s ease-out ${index * 0.1}s both`
                }}
              >
                <div className="flex items-center gap-6">
                  <div className="flex-shrink-0">
                    <div className="w-14 h-14 bg-gradient-to-br from-blue-500 via-purple-500 to-pink-500 rounded-xl flex items-center justify-center shadow-lg group-hover:shadow-purple-500/50 transition-shadow duration-300">
                      <span className="text-white text-xl font-bold">{item.id}</span>
                    </div>
                  </div>
                  
                  <div className="flex-grow">
                    <div className="flex items-center gap-3 mb-1">
                      <span className="text-xs font-semibold text-gray-400 uppercase tracking-wider">ID: {item.id}</span>
                      <span className="px-2 py-1 bg-green-500/20 text-green-300 text-xs font-medium rounded-full">Active</span>
                    </div>
                    <p className="text-white text-lg font-medium group-hover:text-purple-300 transition-colors duration-300">
                      {item.text}
                    </p>
                  </div>

                  <div className="flex-shrink-0">
                    <button className="p-2 text-gray-400 hover:text-white hover:bg-white/10 rounded-lg transition-all duration-300">
                      <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                      </svg>
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Footer */}
        <div className="text-center mt-12">
          <p className="text-gray-400 text-sm">
            Displaying {items.length} items • Built with React & Tailwind CSS
          </p>
        </div>
      </div>
    </div>
  )
}

export default App
