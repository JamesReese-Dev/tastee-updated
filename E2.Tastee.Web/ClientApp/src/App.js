import React, { useState, useContext, useEffect } from "react";
import { Route, Switch, Redirect } from "react-router-dom";
import _ from "lodash";
import Alert from "react-s-alert";
import "react-s-alert/dist/s-alert-default.css";
import "react-s-alert/dist/s-alert-css-effects/slide.css";
import { api, constants, helpers, UserContext, storage} from "./utils";
import { Footer, SidebarMenu } from "./layouts";
import {
  Admin,
  Home,
  UserProfile,
  SignIn,
  User,
  Users,
  ReferenceDataList,
  ResetPassword,
  ForgotPassword,
  UpdatePassword
} from "./components";

const nowTime = () => new Date().getTime();
const { THEMES } = constants;

export default function App() {
  const userCtx = useContext(UserContext);
  const tokenFromStorage = api.userToken() || null;
  const parsedUser = tokenFromStorage
    ? JSON.parse(storage.getItem("currentUser"))
    : null;
  const [authToken, setAuthToken] = useState(tokenFromStorage);
  const [currentUser, setCurrentUser] = useState(parsedUser);
  const [theme, setTheme] = useState(THEMES.default);
  const [alertMessage, setAlertMessage] = useState(null);
  const [onPublicPage, setOnPublicPage] = useState(true);

  function isAuthenticated() {
    return authToken !== null;
  }

  function updateUserContext(userRole) {
    api
      .post("UserAdmin/UpdateCurrentUserContext", {
        UserRole: userRole,
        User: currentUser,
      })
      .then((r) => {
        if (r?.data?.user) {
          let newRoleUser = r.data.user;
          storage.setItem("currentUser", JSON.stringify(newRoleUser));
          setCurrentUser(newRoleUser);
          return <Redirect to="/admin/menu" push={true} />;
        }
      });
  }

  function enhanceUser(u) {
    if (!u) return u;
    // u.includeHaulerFirmModule = u.isHaulerAdminUser;
    if (u.token) {
      delete u.token;
    }
    return u;
  }

  function signOut(message) {
     api.post("public/recordLogoff").finally(() => {
       storage.removeItem("token");
       storage.removeItem("currentUser");
       storage.removeItem("lastActivity");
       storage.removeItem("sessionStartedAt");
       setCurrentUser(null);
       setAuthToken(null);
       if (message) {
         setAlertMessage(message);
       }

       //if (impersonating) {
       //  clearImpersonation();
       //}
     });
  }

  const AuthRoute = ({ component: Component, ...extraProps }) => {
    return (
      <Route
        {...extraProps}
        render={(props) => {
          setOnPublicPage(false);
          const combinedProps = Object.assign(props, extraProps);
          if (!authToken) {
            setOnPublicPage(true);
            return <Redirect to="/" />;
          }
          if (!isAuthenticated()) {
            setOnPublicPage(true);
            return <Redirect to="/" />;
          }
          return storage.getItem("untethering") ? (
            unSetTetheringAndRedirect(extraProps.location.pathname)
          ) : (
            <Component {...combinedProps} />
          );
        }}
      />
    );
  };

  const LoginRoute = ({ component: Component, ...extraProps }) => {
    return (
      <Route
        {...extraProps}
        render={(props) => {
          const combinedProps = Object.assign(props, extraProps);
          if (isAuthenticated()) {
            setOnPublicPage(false);
            return <Redirect to="/admin/menu" />;
          }
          if (
            (_.startsWith(combinedProps.path, "/reset_password") ||
              _.startsWith(combinedProps.path, "/forgot_password")) &&
            Component
          ) {
            setOnPublicPage(true);
            return <Component {...combinedProps} />;
          }
          setOnPublicPage(true);
          return <SignIn {...combinedProps} />;
        }}
      />
    );
  };

  function unSetTetheringAndRedirect(path) {
    storage.removeItem("untethering");
    return <Redirect to="/" />;
  }
  const referencePathList = _.map(
    constants.REFERENCE_DATA_URL_LIST,
    (x) => x.reactPath
  );
  const showSidebar =
    userCtx && currentUser && currentUser.id;

  const user = {
    currentUser: currentUser,
    setCurrentUserContext: updateUserContext,
    theme: theme,
    setTheme: setTheme,
    signIn: (newUser, token) => {
      if (token) {
        storage.setItem("token", token);
        setAuthToken(token);
      }
      newUser = enhanceUser(newUser);
      storage.setItem("lastUsername", newUser.username);
      storage.setItem("currentUser", JSON.stringify(newUser));
      storage.setItem("sessionStartedAt", nowTime());
      setCurrentUser(newUser);
      setAlertMessage(null);
    },
    signOut: signOut
  };

  return (
    <div className={`${theme} siteContainer fullHeight`}>
      <Alert
        effect="slide"
        position="top-right"
        stack={{ limit: 1 }}
        timeout={4000}
        html={true}
        offset={1}
        z-index={4000}
        preserveContext
      />
      <UserContext.Provider value={user}>
        {showSidebar ? <SidebarMenu /> : null}
        <div>
          <Switch>
            <LoginRoute exact path="/login" />
            <LoginRoute
              path="/forgot_password"
              component={ForgotPassword}/>
            <LoginRoute
              path="/reset_password/:resetToken"
              component={ResetPassword}
            />

            <Route exact path="/" component={Home} />

            <AuthRoute exact path="/user/:id" component={User} />
            <AuthRoute exact path="/admin/users" component={Users} />
            <AuthRoute exact path="/admin/:tabName" component={Admin} />
            <AuthRoute exact path="/admin/menu" component={Admin} />
            <AuthRoute exact path="/profile" component={UserProfile} />
            <AuthRoute exact path="/dashboard" component={Home} />
            <AuthRoute
              exact
              path={referencePathList}
              component={ReferenceDataList}
            />
          </Switch>
        </div>
        <Footer />
      </UserContext.Provider>
    </div>
  );
}
