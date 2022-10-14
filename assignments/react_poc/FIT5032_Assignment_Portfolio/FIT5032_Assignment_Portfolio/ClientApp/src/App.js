import React, { Component, Fragment } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths, LoginActions, LogoutActions } from './components/api-authorization/ApiAuthorizationConstants';
import { Login } from './components/api-authorization/Login'
import { Logout } from './components/api-authorization/Logout'

import './custom.css';

export default class App extends Component {
  static displayName = App.name;

    render() {


        function loginAction(name) {
            return (<Login action={name}></Login>);
        }

        function logoutAction(name) {
            return (<Logout action={name}></Logout>);
        }
    return (
      <Layout>
        <Routes>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}

                <Route path={ApplicationPaths.Login} element={loginAction(LoginActions.Login)} />
                <Route path={ApplicationPaths.LoginFailed} element={loginAction(LoginActions.LoginFailed)} />
                <Route path={ApplicationPaths.LoginCallback} element={loginAction(LoginActions.LoginCallback)} />
                <Route path={ApplicationPaths.Profile} element={loginAction(LoginActions.Profile)} />
                <Route path={ApplicationPaths.Register} element={loginAction(LoginActions.Register)} />
                <Route path={ApplicationPaths.LogOut} element={logoutAction(LogoutActions.Logout)} />
                <Route path={ApplicationPaths.LogOutCallback} element={logoutAction(LogoutActions.LogoutCallback)} />
                <Route path={ApplicationPaths.LoggedOut} element={logoutAction(LogoutActions.LoggedOut)} />
        </Routes>
      </Layout>
    );
  }
}