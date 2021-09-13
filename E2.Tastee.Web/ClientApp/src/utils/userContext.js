import React from "react";

const UserContext = React.createContext({
  currentUser: null,
  theme: null,
  // authCurrentUser: i => {},  
  setTheme: i => {},
  signIn: (user, token) => {},
  signOut: i => {},
  impersonate: i => {},
  clearImpersonation: i => { },
  company: null,
  haulerFirm: null,
//  currentUserContext: null,
//  setCurrentUserContext: i => { },
});

export default UserContext;
