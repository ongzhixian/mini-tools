
var GHPATH = '/mini-tools';

// Choose a different app prefix name
var APP_PREFIX = 'mtghp_';

// The version of the cache. Every time you change any of the files
// you need to change this version (version_01, version_02…). 
// If you don't change the version, the service worker will give your
// users the old files!
var VERSION = 'version_07';

const cacheName = `${APP_PREFIX}${VERSION}`;

// The files to make available for offline use. make sure to add 
// others to this list
var URLS = [
    `${GHPATH}/`,
    `${GHPATH}/index.html`,
    `${GHPATH}/css/normalize.css`,
    `${GHPATH}/css/skeleton.css`,
    `${GHPATH}/images/favicon.png`,
    `${GHPATH}/js/app.js`,
    "//fonts.googleapis.com/css?family=Raleway:400,300,600"
]

const contentToCache = URLS; // appShellFiles.concat(gamesImages)

console.log(...contentToCache);

// Installing Service Worker
self.addEventListener('install', (e) => {
    console.log('[Service Worker] Install');
    e.waitUntil((async () => {
        const cache = await caches.open(cacheName);
        console.log('[Service Worker] Caching all: app shell and content');
        await cache.addAll(contentToCache);
    })());
});

self.addEventListener( "activate", event => {
    console.log('WORKER: activate event in progress.');
});

// Fetching content using Service Worker
self.addEventListener('fetch', (e) => {
    e.respondWith((async () => {
        const r = await caches.match(e.request);
        console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
        if (r) return r;
        const response = await fetch(e.request);
        const cache = await caches.open(cacheName);
        console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
        cache.put(e.request, response.clone());
        return response;
    })());
});