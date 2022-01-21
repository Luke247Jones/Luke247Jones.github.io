import React, { Component, useEffect } from 'react';
import { Platform, TouchableOpacity, Text, TextInput, StyleSheet, View, Modal, Image } from 'react-native';
import {Container, Content, Spinner} from 'native-base';
import AsyncStorage from '@react-native-community/async-storage';

//Check for valid token in AsyncStorage
const StartUpScreen = (props) => {
  const tryLogin = async () => {
    const userData = await AsyncStorage.getItem('userData');
    //Navigiate to Login screen if there is no local storage 'userData' storing the token
    if(!userData)
    {
      props.navigation.navigate({routeName: 'Login'});
      return;
    }
    const parseData = JSON.parse(userData);
    const token = parseData.token;
    //Check API to see if token is valid or expired
    const axios = require('axios').default;
    const url = 'https://cognifyxapp.herokuapp.com/tokenexpired?token=' + token;
    axios.post(url)
      .then(response => {
        if(JSON.stringify(response.data).includes("false"))
        {
          props.navigation.navigate({routeName: 'HomeScreen'});
          console.log(response.data);
        }
        else
        {
          props.navigation.navigate({routeName: 'Login'});
        }
  })
}
  useEffect(() => {
    tryLogin();
  }, []);

  //UI for Starting app screen
  return (
    <Container>
        <View style={{flex:1, justifyContent: 'center', alignItems: 'center'}}>
          <View style={{flexDirection: 'row', alignItems: 'center'}}>
            <Text styyle={{fontSize: 14}}> Starting  </Text>
            <Spinner color="grey"/>
          </View>
        </View>
    </Container>
  );
};

export default StartUpScreen;
