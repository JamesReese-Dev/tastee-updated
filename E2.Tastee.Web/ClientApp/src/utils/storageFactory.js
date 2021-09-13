let inMemoryStorage = {};

const isSupported = () => {
  try {
    const testKey = "teststorage";
    localStorage.setItem(testKey, testKey);
    localStorage.removeItem(testKey);
    return true;
  } catch (e) {
    return false;
  }
};

const clear = () => {
  if (isSupported()) {
    localStorage.clear();
  } else {
    inMemoryStorage = {};
  }
};

const getItem = (k) => {
  if (isSupported()) {
    return localStorage.getItem(k);
  }
  if (inMemoryStorage.hasOwnProperty(k)) {
    return inMemoryStorage[k];
  }
  return null;
};

const key = (idx) => {
  if (isSupported()) {
    return localStorage.key(idx);
  }
  return Object.keys(inMemoryStorage)[idx] || null;
};

const removeItem = (k) => {
  if (isSupported()) {
    localStorage.removeItem(k);
  } else {
    delete inMemoryStorage[k];
  }
};

const setItem = (k, v) => {
  if (isSupported()) {
    localStorage.setItem(k, v);
  } else {
    inMemoryStorage[k] = String(v);
  }
};

const length = () => {
  if (isSupported()) {
    return localStorage.length;
  }
  return Object.keys(inMemoryStorage).length;
};

export default {
  getItem,
  setItem,
  removeItem,
  clear,
  key,
  length
};
