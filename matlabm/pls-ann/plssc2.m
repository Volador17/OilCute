function [XScores ]=plssc2(X,Loads,Weights,b,Score_length);
% Ŀ�꣺�����ؼ��÷�
% ������������� 
%            X              ���׾���                        [�������� * δ֪�Ĳ��׸���(������)]
%            Scores         У������ͼ�÷־���               [��Ʒ��*���ɷ���]
%            Loads          У������ͼ�غɾ���               [������*���ɷ���]
%            Weights        У������ͼȨ�ؾ���               [������*���ɷ���]
%            b              У�������Իع麯��               [1 * ���ɷ���]
%            Score_length   �÷������ĳ���                   [1 * ���ɷ���]
% �����������:	
%            XScores   δ֪�����ĵ÷־���       [ δ֪���ո��� * ���ɷָ��� ]
% ��������: ASTM ��׼E 1655-05 ��11ҳ PLS 
[RowsX,ColsX] = size (X);
[RowsLoads,ColsLoads]=size(Loads); 
% ��ʼ������������� ���о������Ϊk �������Ϊn
XScores =zeros (ColsX,ColsLoads);
Factor=ColsLoads;
% �����ǲ��� ASTM �ϵ�11ҳע�ͽ��У��ܹ�˳����ɶ� δ֪������Ԥ�� �������޸ĺ�ͨ�� Scores ���й���
for i = 1 : Factor
    Weights(:,i)=Weights(:,i)/Score_length(i);
    XScores(:,i)= X'*Weights(:,i);
    XScores(:,i)=XScores(:,i)/Score_length(i);
end




