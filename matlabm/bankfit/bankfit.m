function [ratio,ad]=bankfit(x,y);
% �������Ͻ�����֤
% ���룺x������Ʒ���ף�y���������Ʒ���ף�
% �����ad��������ϵĿ������Ʒ��ţ�ratio��������ϵĿ���׵���Ӧ������
	[ra] = spectrafit(x,y);
	ra=ra';
	[B,INDEX]=sort(ra);
	ad1=find(B>0); 
	[m1,n1]=size(ad1);
	[m2,n2]=size(B);
	for i=1:n1
		ratio(i)=B(n2-i+1);
		ad(i)=INDEX(n2-i+1);
	end
	
	ratio=ratio'/sum(ratio);
	ad = ad';
end
