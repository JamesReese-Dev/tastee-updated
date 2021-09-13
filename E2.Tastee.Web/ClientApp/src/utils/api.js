import axios from "axios";
import Alert from "react-s-alert";
import _ from "lodash";
import { storage } from "./";
const API = "/api";

function userToken() {
  return storage.getItem("token");
}

function commonHeaders() {
  const token = storage.getItem("token");
  return {
    Accept: "application/json",
    Authorization: `Bearer ${token}`,
  };
}

const headers = () =>
  Object.assign(commonHeaders(), { "Content-Type": "application/json" });

const multipart_headers = () =>
  Object.assign(commonHeaders(), { "Content-Type": "multipart/form-data" });

const html_content_type_headers = () =>
  Object.assign({}, {
    // "Accept": "application/octet-stream",
    "Access-Control-Allow-Origin": "*",
    "Content-Type": "application/octet-stream"
  });

function queryString(params) {
  const query = Object.keys(params)
    .map((k) => `${encodeURIComponent(k)}=${encodeURIComponent(params[k])}`)
    .join("&");
  return `${query.length ? "?" : ""}${query}`;
}

axios.interceptors.response.use(
  function (response) {
    const clientVersion = storage.getItem("app-version");
    const serverVersion = response.headers["zw-version"];
    if (clientVersion === null || clientVersion !== serverVersion) {
      if (clientVersion !== undefined && serverVersion !== undefined) {
        storage.setItem("app-version", serverVersion);
        console.warn("This page is about to reload with an updated version...");
        setTimeout(function () {
          // try {
          //   unregister();
          // } catch {
          window.location.reload(true);
          // }
        }, 300);
      }
    }
    return response;
  },
  function (error) {
    if (error.response && error.response.status === 401) {
      console.log("Unauthorized - redirecting to login");
      // storage.removeItem("loggedOnAt");
      storage.removeItem("currentUser");
      storage.removeItem("currentParticipant");
      storage.removeItem("token");
      setTimeout(function () {
        window.location.href = "/";
      }, 500);
    }
    if (error.response && error.response.status === 403) {
      Alert.error(
        "Your user account does not have the permissions to take this action"
      );
    }
    return error;
  }
);

const post = (url, data) => {
  return axios({
    method: "post",
    url: `${API}/${url}`,
    data: data,
    timeout: 600000,
    headers: headers(),
  });
};

const apiFetch = (url, params = {}) => {
  return axios.get(`${API}/${url}${queryString(params)}`, {
    headers: headers(),
  });
};

const getTimezones = () =>
  apiFetch("Reference/TimezoneList").then((r) => ({
    timezones: _.map(r.data, (x) => ({ value: x, label: x })),
  }));

const getTenants = () =>
  apiFetch("Reference/TenantList").then((r) => ({
    tenants: _.map(r.data, (x) => ({ ...x, value: x.id, label: x.name })),
  }));

const getMGs = () =>
  api.post("Reference/MGs", {}).then((r) => ({
    MGs: _.map(
      _.reject(r.data.list, (e) => e.id <= 0),
      (e) => ({ value: e.id, label: e.name })
    ),
  }));

const getCGs = () =>
  api.post("Reference/CGs", {}).then((r) => ({
    CGs: _.map(
      _.reject(r.data.list, (e) => e.id <= 0),
      (e) => ({ value: e.id, label: e.name })
    ),
  }));

const getCategories = () =>
  api.post("Reference/Categories", {}).then((r) => ({
    categories: _.map(
      _.reject(r.data.list, (e) => e.id <= 0),
      (e) => ({ value: e.id, label: `${e.cgName}: ${e.name}` })
    ),
  }));

const getSubcategories = () =>
  api.post("Reference/Subcategories", {}).then((r) => ({
    subcategories: _.map(
      _.reject(r.data.list, (e) => e.id <= 0),
      (e) => ({
        value: e.id,
        label: `${e.cgName}: ${e.categoryName}: ${e.name}`,
      })
    ),
  }));

// const getStates = () => apiFetch('Public/StateList').then(r => ({
//   states: _.map(r.data, x => ({...x, value: x.id, label: x.name}))
// }));

const api = {
  // downloadDocument(url) {
  //   const bol_1 = img1;
  //   const bol_2 = img2;
  //   const bol_3 = img3;
  //   const goodPallet = img4;
  //   const leaningPallet = img5;
  //   const pallets = img6;

  //   const doc_url = eval(url);
  //   try {
  //     var link = document.createElement("a");
  //     link.setAttribute("href", doc_url);
  //     link.setAttribute("download", doc_url);
  //     link.style.visibility = "hidden";
  //     document.body.appendChild(link);
  //     link.click();
  //     document.body.removeChild(link);
  //   } catch {
  //     Alert.error("There was an error downloading the document");
  //   }
  // },
  // downloadCSV(url) {
  //   const quantity_of_shipments = csv1;
  //   const quality_assurance_and_compliance = csv2;

  //   const doc_url = eval(url);
  //   try {
  //     var link = document.createElement("a");
  //     link.setAttribute("href", doc_url);
  //     link.setAttribute("download", doc_url);
  //     link.style.visibility = "hidden";
  //     document.body.appendChild(link);
  //     link.click();
  //     document.body.removeChild(link);
  //   } catch {
  //     Alert.error("There was an error downloading the document");
  //   }
  // },
  // downloadPDF(url) {
  //   try {
  //     var link = document.createElement("a");
  //     // link.setAttribute("href", url);
  //     // link.setAttribute("download", url);
  //     // just for demo ______________
  //     link.setAttribute("href", pdf1);
  //     link.setAttribute("download", pdf1);
  //     link.style.visibility = "hidden";
  //     document.body.appendChild(link);
  //     link.click();
  //     document.body.removeChild(link);
  //   } catch {
  //     Alert.error("There was an error downloading the document");
  //   }
  // },
  get_html_content(url) {
    try {
      if (url) {
        return axios.get(`${url}`, { headers: html_content_type_headers() });
      }
      return Promise.resolve(null);
    } catch {
      return Promise.resolve(null);
    }
  },
  fetch_raw(url, params = {}) {
    return axios.get(`${url}${queryString(params)}`, { headers: headers() });
  },

  fetch: apiFetch,

  upload(verb, url, data) {
    switch (verb.toLowerCase()) {
      case "post":
        return axios.post(`${API}/${url}`, data, { headers: headers() });
      case "put":
        return axios.put(`${API}/${url}`, data, { headers: headers() });
      case "patch":
        return axios.patch(`${API}/${url}`, data, { headers: headers() });
      default:
    }
  },

  post: post,

  put(url, data) {
    return axios.put(`${API}/${url}`, data, { headers: headers() });
  },

  patch(url, data) {
    return axios.patch(`${API}/${url}`, data, { headers: headers() });
  },

  delete(url) {
    return axios.delete(`${API}/${url}`, { headers: headers() });
  },

  post_form_data(url, formData) {
    return axios.post(`${API}/${url}`, formData, {
      headers: multipart_headers(),
    });
  },

  put_form_data(url, formData) {
    return axios.put(`${API}/${url}`, formData, {
      headers: multipart_headers(),
    });
  },

  postGetBlob(url, data) {
    return axios.post(`${API}${url}`, data, {
      headers: headers(),
      withCredentials: true,
      responseType: "blob",
    });
  },

  postFormDataGetBlob(url, formData) {
    return axios.post(`${API}${url}`, formData, {
      // headers: {
      //   // "Content-Type": "multipart/form-data",
      //   Authorization: `Bearer ${userToken()}`
      // },
      headers: multipart_headers(),
      withCredentials: true,
      responseType: "blob",
    });
  },

  // post_form_data_get_blob(url, formData) {
  //   return axios.post(`${API}/${url}`, formData, {
  //     headers: multipart_headers(),
  //     responseType: "blob",
  //   });
  // },

  getTenants: getTenants,
  getMGs: getMGs,
  getCGs: getCGs,
  getCategories: getCategories,
  getSubcategories: getSubcategories,
  getTimezones: getTimezones,
  userToken() {
    return userToken();
  },
  catchHandler: (e) => {
    console.error(e);
  },
};

export default api;
