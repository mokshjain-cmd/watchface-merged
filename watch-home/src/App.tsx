import { useState, useEffect } from 'react'
import { Watch as WatchIcon } from 'lucide-react'
import { Watch, WatchRoute } from './types'
import './App.css'

// Route configuration for different watch types
const WATCH_ROUTES: WatchRoute[] = [
  {
    vendor: 'ZH',
    board: 'JL',
    path: '../ZHJL WatchFace/watch-assembly-tool/dist/index.html',
    port: 5174
  },
  {
    vendor: 'ZH',
    board: 'SF',
    path: '../ZhouHaiWatchFace/frontend/watch-assembly-tool/dist/index.html',
    port: 5175
  },
  {
    vendor: 'MY',
    board: 'JL',
    path: '/Users/mokshjain/Desktop/WFT-MY-JL/index.html',
    port: 5176
  }
]

function App() {
  const [watches, setWatches] = useState<Watch[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    loadWatches()
  }, [])

  const loadWatches = async () => {
    try {
      // Fetch watches from the backend API
      const response = await fetch('http://localhost:3002/api/watches')
      
      if (!response.ok) {
        throw new Error(`Failed to fetch watches: ${response.statusText}`)
      }
      
      const data = await response.json()
      setWatches(data.watches)
      setLoading(false)
    } catch (err) {
      console.error('Error loading watches:', err)
      setError(`Failed to load watches. Make sure the server is running on port 3002.`)
      setLoading(false)
    }
  }

  const handleWatchClick = (watch: Watch) => {
    const route = WATCH_ROUTES.find(
      r => r.vendor === watch.vendor && r.board === watch.board
    )

    if (!route) {
      alert(`No route configured for ${watch.vendor}-${watch.board}`)
      return
    }

    // Build URL with width and height parameters
    let url = `http://localhost:${route.port}`
    if (watch.width && watch.height) {
      url += `?width=${watch.width}&height=${watch.height}&shape=${encodeURIComponent(watch.shape || '')}`
    }
    
    // Open the corresponding frontend in a new window
    window.open(url, '_blank')
    
    // Show instruction to user
    console.log(`Opening ${watch.name} (${watch.vendor}-${watch.board})`)
    console.log(`Platform size: ${watch.width}x${watch.height}`)
    console.log(`Shape: ${watch.shape}`)
    console.log(`Make sure the development server is running on port ${route.port}`)
    console.log(`Run: cd "${route.path.replace('/dist/index.html', '')}" && npm run dev`)
  }

  const getWatchGradient = (index: number) => {
    const gradients = [
      'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
      'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
      'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
      'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
      'linear-gradient(135deg, #30cfd0 0%, #330867 100%)',
      'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)',
      'linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%)',
      'linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%)',
    ]
    return gradients[index % gradients.length]
  }

  if (loading) {
    return (
      <div className="app">
        <div className="loading">Loading watches...</div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="app">
        <div className="error">{error}</div>
      </div>
    )
  }

  return (
    <div className="app">
      <header className="header">
        <div className="logo-container">
          <img src="/Noise Logo_ (1).png" alt="Noise Logo" className="logo" />
        </div>
        <h1>Watch Face Assembly Tool</h1>
        <p>Select a watch to start assembling watch faces</p>
      </header>

      <div className="watch-grid">
        {watches.map((watch, index) => (
          <div
            key={`${watch.name}-${index}`}
            className="watch-card"
            onClick={() => handleWatchClick(watch)}
          >
            <div 
              className="watch-image"
              style={{ 
                background: watch.image ? 'transparent' : getWatchGradient(index)
              }}
            >
              {watch.image ? (
                <img 
                  src={watch.image} 
                  alt={watch.name}
                  className="watch-image-img"
                />
              ) : (
                <WatchIcon className="watch-icon" size={64} />
              )}
            </div>
            <div className="watch-info">
              <div className="watch-name">{watch.name}</div>
              <div className="watch-details">
                {watch.width && watch.height && (
                  <span className="watch-badge badge-size">{watch.width}×{watch.height}</span>
                )}
                {watch.shape && (
                  <span className="watch-badge badge-shape">{watch.shape}</span>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default App
