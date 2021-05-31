import React, { Component, useState } from 'react';
import { Text, View, StyleSheet, FlatList, Image } from 'react-native';
import { createAppContainer } from 'react-navigation';
import { createStackNavigator } from 'react-navigation-stack';

import AppNavigator from './navigation/AppNavigation.js';



export default function App() {

  return <AppNavigator/>;

}
