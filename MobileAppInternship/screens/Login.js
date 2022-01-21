import React, { Component } from 'react';
import { Ionicons } from '@expo/vector-icons';
import Icon1 from 'react-native-vector-icons/FontAwesome5';
import Icon2 from 'react-native-vector-icons/Entypo';
import Icon3 from 'react-native-vector-icons/Fontisto';
import Icon4 from 'react-native-vector-icons/SimpleLineIcons';
import { Platform, TouchableOpacity, Text, TextInput, StyleSheet, View, Modal, Image } from 'react-native';
import { Container, Header, Content, Form, Item, Input, Label, DatePicker, Picker} from 'native-base';
import AsyncStorage from '@react-native-community/async-storage';
import * as Font from 'expo-font';
import { loginCheck } from '../storage/ErrorCheck.js';

export default class Login extends Component{
  constructor(props) {
    super(props);
    //States for user input and error output alert.
    this.state = {
      username: '',
      password: '',
      loginAlert: '',
      lineColor: "#aaaaaa",
      placeholderColor: "#aaaaaa",
    }
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  //Check login for frontend error and set alert.
  handleSubmit() {
    this.setState({loginAlert: loginCheck(this.state)}, this.sendSubmit);
  }

  //Send login information for backend validation if frontend error alert is empty.
  sendSubmit = () => {
    const {loginAlert} = this.state;

    if(loginAlert == '')
    {
      this.validateLogin();
    }
    else
    {
      this.setState({lineColor: 'red'})
      this.setState({placeholderColor: 'red'})
    }
  };

  //Send data to API to see if username and password are accepted.
  //Navigate to home screen if they are. Set error alert if they aren't.
  validateLogin = async function() {
    const axios = require('axios').default;
    const data = {
      "username": this.state.username,
      "password": this.state.password
    }
    console.log(data);
    axios({
      method: 'post',
      url: 'https://cognifyxapp.herokuapp.com/login',
      data: data,
      validateStatus: false
      }).then(response => {
        if(JSON.stringify(response.data).includes("token"))
        {
          const data = JSON.stringify(response.data);
          const parseData = JSON.parse(data);
          this.saveDataToStorage(parseData.token, parseData.userId, this.state.username);
          this.props.navigation.navigate({routeName: 'HomeScreen'});
        }
        else
        {
          this.setState({loginAlert: "Username or Password is incorrect"})
          this.setState({lineColor: 'red'})
          this.setState({placeholderColor: 'red'})
        }
      })
  }
  //Save token, userId, and username to local storage
  saveDataToStorage = (token, userId, username) => {
    AsyncStorage.setItem('userData', JSON.stringify({token: token, userId: userId, username: username}));
  };

  //UI for Login screen
  render() {
    return (
      <View transparent= 'true' style= {styles.container}>
        <Content style= {styles.content}>
          <Image source = {{uri:'https://pbs.twimg.com/profile_images/486929358120964097/gNLINY67_400x400.png'}} style = {styles.image}/>
          <View style= {styles.title}>
            <Text style= {styles.textTitle}>Please login with your details</Text>
          </View>
          <Form style= {styles.form}>
            <View style= {styles.error}>
              <Text style= {styles.textError}> {this.state.loginAlert} </Text>
            </View>
            <View style= {[styles.item, {borderColor: this.state.lineColor}]}>
              <Icon2 name="user" color= '#ee5566' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {"Username"}
                placeholderTextColor= {this.state.placeholderColor}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    username: text
                  };
                })}}
                maxLength= {32}
              />
            </View>
            <View>
              <Text style= {styles.textError}> {this.state.usernameAlert} </Text>
            </View>
            <View style= {[styles.item, {borderColor: this.state.lineColor}]}>
              <Icon1 name="lock" color= 'black' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {" Password"}
                secureTextEntry= {true}
                placeholderTextColor= {this.state.placeholderColor}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    password: text
                  };
                })}}
                maxLength= {32}
              />
            </View>
            <View>
              <Text style= {styles.textError}> {this.state.passwordAlert} </Text>
            </View>
          </Form>
          <View style= {styles.buttons}>
              <View style= {styles.login}>
              <TouchableOpacity
                style= {styles.login}
                onPress={this.handleSubmit}
              >
                <Text style= {styles.textLogin}> Login </Text>
              </TouchableOpacity>
              </View>
              <View style= {styles.signup}>
                <Text style= {styles.textSignUp}> Don't have an account? </Text>
                <TouchableOpacity
                  onPress= {() => this.props.navigation.navigate({routeName: 'SignUp'})}
                >
                  <Text style= {styles.textSignUpHere}> Sign Up Here </Text>
                </TouchableOpacity>
              </View>
          </View>
          <Text> {this.state.output} </Text>
        </Content>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#ffffff'
  },
  content: {
    paddingTop: 10,
    backgroundColor: 'white'
  },
  form: {
    //paddingTop: 10,
    backgroundColor: 'white'
  },
  error: {
    paddingTop: 20,
    paddingBottom: 20,
    alignItems: 'center'
  },
  item: {
    flexDirection: 'row',
    justifyContent: 'flex-start',
    alignItems: 'center',
    borderBottomWidth: 1,
    margin: 15,
    paddingLeft: 10,
  },
  input: {
    paddingLeft: 27,
    backgroundColor: 'white'
  },
  title: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignContent: 'center'
  },
  buttons: {
    flex: 1,
    padding: 20,
    paddingTop: 30,
    alignItems: 'center',
    //alignContent: 'space-between',
    justifyContent: 'center'
  },
  login: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: "center",
    backgroundColor: "#77bb77",
    //padding: 10,
    height: 70,
    width: 180,
    //flex: 1,
    borderRadius: 50,
    borderWidth: 2,
    borderColor: "#77bb77",
    shadowColor: "black",
    shadowRadius: 8,
    shadowOpacity: 0.15
  },
  signup: {
    flexDirection: 'row',
    paddingTop: 30
  },
  textTitle:{
    fontSize: 25,
    fontWeight: 'bold',
    color: '#555555',
    paddingTop: 20
  },
  textLogin: {
    fontSize: 25,
    fontWeight: 'bold',
    color: 'white'
  },
  textSignUp: {
    fontSize: 20
  },
  textSignUpHere: {
    fontSize: 20,
    fontWeight: 'bold',
    color: "#77bb77"
  },
  textError: {
    color: 'red',
    fontSize: 16
  },
  image: {
    width: 250,
    height: 250,
    justifyContent: 'center',
    alignSelf: 'center'

  }
});
